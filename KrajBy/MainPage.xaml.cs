using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.IO;
using System.Net.NetworkInformation;

using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Scheduler;

namespace KrajBy
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string AgentName = "ForTile";

        // Города
        string[] CityNames = { 
                            "Браслав",    // 0
                            "Вилейка",    // 1
                            "Воложин",    // 2
                            "Глубокое",   // 3
                            "Докшицы",    // 4
                            "Логойск",    // 5
                            "Молодечно",  // 6
                            "Мядель",     // 7
                            "Островец",   // 8
                            "Ошмяны",     // 9
                            "Поставы",    // 10
                            "Сморгонь"    // 11
                        };        

        const string cNames= "<?xml version='1.0' encoding='utf-8' ?> <rss version='2.0'>" +
					    "<channel>" +
				        "<language>ru-ru</language>" +
                        "<item> <cName>Браслав</cName> <cImage>/Img/city/braslav.png</cImage><gCol>0</gCol><gRow>0</gRow> </item>" +
                        "<item> <cName>Вилейка</cName> <cImage>/Img/city/vileika.png</cImage><gCol>1</gCol><gRow>0</gRow> </item>" +
                        "<item> <cName>Воложин</cName> <cImage>/Img/city/volozin.png</cImage><gCol>0</gCol><gRow>1</gRow> </item>" +
                        "<item> <cName>Глубокое</cName> <cImage>/Img/city/glubokoe.png</cImage><gCol>1</gCol><gRow>1</gRow> </item>" +
                        "<item> <cName>Докшицы</cName> <cImage>/Img/city/dokshisi.png</cImage><gCol>0</gCol><gRow>2</gRow> </item>" +
                        "<item> <cName>Логойск</cName> <cImage>/Img/city/logoisk.png</cImage><gCol>1</gCol><gRow>2</gRow> </item>" +
                        "<item> <cName>Молодечно</cName> <cImage>/Img/city/molodechno.png</cImage><gCol>0</gCol><gRow>3</gRow> </item>" +
                        "<item> <cName>Мядель</cName> <cImage>/Img/city/myadel.png</cImage><gCol>1</gCol><gRow>3</gRow> </item>" +
                        "<item> <cName>Островец</cName> <cImage>/Img/city/ostrovets.png</cImage><gCol>0</gCol><gRow>4</gRow> </item>" +
                        "<item> <cName>Ошмяны</cName> <cImage>/Img/city/oshmyani.png</cImage><gCol>1</gCol><gRow>4</gRow> </item>" +
                        "<item> <cName>Поставы</cName> <cImage>/Img/city/postavi.png</cImage><gCol>0</gCol><gRow>5</gRow> </item>" +
                        "<item> <cName>Сморгонь</cName> <cImage>/Img/city/smorgon.png</cImage><gCol>1</gCol><gRow>5</gRow> </item>" +
                        "</channel>" +
                        "</rss>";
        // Для погоды
        string[] RSSs = { 
                            "http://weather.yahooapis.com/forecastrss?w=824376&u=c",  // Браслав     0
                            "http://weather.yahooapis.com/forecastrss?w=833010&u=c", // Вилейка     1
                            "http://weather.yahooapis.com/forecastrss?w=832766&u=c", // Воложин     2
                            "http://weather.yahooapis.com/forecastrss?w=825516&u=c",  // Глубокое    3
                            "http://weather.yahooapis.com/forecastrss?w=824945&u=c",  // Докшицы     4
                            "http://weather.yahooapis.com/forecastrss?w=827788&u=c", // Логойск     5
                            "http://weather.yahooapis.com/forecastrss?w=828110&u=c", // Молодечно   6
                            "http://weather.yahooapis.com/forecastrss?w=828813&u=c", // Мядель      7
                            "http://weather.yahooapis.com/forecastrss?w=829612&u=c",  // Островец    8
                            "http://weather.yahooapis.com/forecastrss?w=823577&u=c",  // Ошмяны      9
                            "http://weather.yahooapis.com/forecastrss?w=829777&u=c",  // Поставы     10
                            "http://weather.yahooapis.com/forecastrss?w=831639&u=c",  // Сморгонь    11
                        };

        string RSS = "http://kraj.by/belarus/news/?rss=articles";
        string RSSFileName = "rss.xml";
        string RSSString = "";
        //bool loaded = false;

        progessOnFront progressOn;

        ApplicationBarIconButton btn0;
        ApplicationBarIconButton btn1;

        Functions allFunc = new Functions();

        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            TiltEffect.SetIsTiltEnabled(this, true);
            btn0 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
        }

        private void LoadRSS()
        {
            //if (loaded) return;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Отсутствует соединение с сетью интернет.");
                return;
            }
            
            btn0.IsEnabled = false;
            btn1.IsEnabled = false;

            if (progressOn == null)
                progressOn = new progessOnFront();

            progressOn.Show(this);
            
            RequestRSS();
            //loaded = true;
        }

        private void RequestRSS()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(RSS));
        }
        
        void ParseRSSAndBindData(string RSSText)
        {
            try
            {
                XElement twitterElements = XElement.Parse(RSSText);

                var postList =
                    from tweet in twitterElements.Descendants("item")
                    select new PostMessage
                    {
                        title = tweet.Element("title").Value,
                        mainImage = allFunc.GetImageFromPostContents(tweet.Element("description").Value),
                        description = Regex.Replace(tweet.Element("description").Value, "<.*?>", String.Empty),
                        pubDate = Convert.ToDateTime(tweet.Element("pubDate").Value).ToString(),
                        link = tweet.Element("link").Value
                    };

                RssList.ItemsSource = postList;
                allFunc.ChangeTile(postList.First<PostMessage>(), 0, true);
            }
            catch
            {
            }
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                RSSString = e.Result;

                ParseRSSAndBindData(RSSString);
            }
            else
            {
                MessageBox.Show("Нет доступа к сайту. Попробуете повторить попытку позже.");
                progressOn.Hide();
                btn0.IsEnabled = true;
                btn1.IsEnabled = true;
                return;
            }
            LoadWeather();
        }

        private string GetDayOfWeek(string enDay)
        {
            if (enDay == "Fri")
                return "Пятница";
            else if (enDay == "Sat")
                return "Суббота";
            else if (enDay == "Sun")
                return "Воскресение";
            else if (enDay == "Mon")
                return "Понедельник";
            else if (enDay == "Tue")
                return "Вторник";
            else if (enDay == "Wed")
                return "Среда";
            else if (enDay == "Thu")
                return "Четверг";
            return "";
        }

        void ParseRSSWAndBindData(string RSSText)
        {
            try
            {
                XElement twitterElements = XElement.Parse(RSSText);

                var postList = twitterElements.Descendants("item");
                var elementItem = postList.Elements<XElement>();
                int i = 0;
                selCityName.Text = CityNames[allFunc.wCity];

                foreach (XElement el in elementItem)
                {
                    if (el.Name.LocalName == "condition")
                        curTemp.Text = el.Attribute("temp").Value + "°C";
                    if (el.Name.LocalName == "forecast")
                    {
                        switch (i)
                        {
                            case 0:
                                curDay.Text = "Сегодня";
                                curImage.Source = new BitmapImage(new Uri("http://us.i1.yimg.com/us.yimg.com/i/us/nws/weather/gr/" + el.Attribute("code").Value + "d.png", UriKind.Absolute));
                                curMin.Text = el.Attribute("low").Value + "°C";
                                curMax.Text = el.Attribute("high").Value + "°C";
                                break;
                            case 1:
                                cur1Day.Text = "Завтра";
                                cur1Image.Source = new BitmapImage(new Uri("http://us.i1.yimg.com/us.yimg.com/i/us/nws/weather/gr/" + el.Attribute("code").Value + "d.png", UriKind.Absolute));
                                cur1Min.Text = el.Attribute("low").Value + "°C";
                                cur1Max.Text = el.Attribute("high").Value + "°C";
                                break;
                            case 2:
                                cur2Day.Text = GetDayOfWeek(el.Attribute("day").Value);
                                cur2Image.Source = new BitmapImage(new Uri("http://us.i1.yimg.com/us.yimg.com/i/us/nws/weather/gr/" + el.Attribute("code").Value + "d.png", UriKind.Absolute));
                                cur2Min.Text = el.Attribute("low").Value + "°C";
                                cur2Max.Text = el.Attribute("high").Value + "°C";
                                break;
                            case 3:
                                cur3Day.Text = GetDayOfWeek(el.Attribute("day").Value);
                                cur3Image.Source = new BitmapImage(new Uri("http://us.i1.yimg.com/us.yimg.com/i/us/nws/weather/gr/" + el.Attribute("code").Value + "d.png", UriKind.Absolute));
                                cur3Min.Text = el.Attribute("low").Value + "°C";
                                cur3Max.Text = el.Attribute("high").Value + "°C";
                                break;
                            case 4:
                                cur4Day.Text = GetDayOfWeek(el.Attribute("day").Value);
                                cur4Image.Source = new BitmapImage(new Uri("http://us.i1.yimg.com/us.yimg.com/i/us/nws/weather/gr/" + el.Attribute("code").Value + "d.png", UriKind.Absolute));
                                cur4Min.Text = el.Attribute("low").Value + "°C";
                                cur4Max.Text = el.Attribute("high").Value + "°C";
                                break;
                        }
                        i++;
                    }
                }
            }
            catch
            {
            }
        }

        private void LoadWeather()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadWeatherCompleted);
            client.DownloadStringAsync(new Uri(RSSs[allFunc.wCity]));
        }

        void client_DownloadWeatherCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                RSSString = e.Result;

                ParseRSSWAndBindData(RSSString);
            }
            else
                MessageBox.Show("Нет доступа к сайту. Попробуете повторить попытку позже.");

            progressOn.Hide();
            btn0.IsEnabled = true;
            btn1.IsEnabled = true;
        }

        private void RssList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
//            if (RssList.SelectedItem != null)
//            {
//                WebBrowserTask webTask = new WebBrowserTask();
//                webTask.Uri = new Uri(((PostMessage)(RssList.SelectedItem)).link);
//                webTask.Show();
//            }
        }

        private void PhoneApplicationPage_Loaded_2(object sender, RoutedEventArgs e)
        {
            // Запускаем агента
            PeriodicTask ForTileTask = ScheduledActionService.Find(AgentName) as PeriodicTask; 
 
            if (ForTileTask != null) 
            { 
                try 
                { 
                    ScheduledActionService.Remove(AgentName); 
                } 
                catch 
                { 
                 //   MessageBox.Show("Невозможно удалить ранее созданного агента:" + ex.Message); 
                } 
            }

            ForTileTask = new PeriodicTask(AgentName);
            ForTileTask.Description = "Описание:" + AgentName; 
 
            try 
            { 
                ScheduledActionService.Add(ForTileTask); 
 
                #if DEBUG 
                    ScheduledActionService.LaunchForTest(AgentName, TimeSpan.FromSeconds(30)); 
                #endif 
            } 
            catch 
            { 
             //   MessageBox.Show("Невозможно создать агента:" + ex.Message); 
            } 

            LoadRSS();
            cList.Children.Clear();

            XElement twitterElements = XElement.Parse(cNames);

            var postList =
                from tweet in twitterElements.Descendants("item")
                select new Cities
                {
                    cName = tweet.Element("cName").Value,
                    cImage = tweet.Element("cImage").Value
                };

            Color currentAccentColorHex = Color.FromArgb(0xFF, 0xDF, 0xD6, 0xAB);
            SolidColorBrush backColor = new SolidColorBrush(currentAccentColorHex);
            Color currentForegroundColor = Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A);
            SolidColorBrush foregroundColor = new SolidColorBrush(currentForegroundColor);
            Thickness lbuttonMargin = new Thickness(0, -7, -7, -7);
            Thickness rbuttonMargin = new Thickness(-7, -7, 0, -7);
            Thickness imageMargin = new Thickness(35, 35, 35, 35);
            Thickness textMargin = new Thickness(10, -30, 0, -30);
            Thickness borderTh = new Thickness(0);
            Thickness buttonMargin = rbuttonMargin;

            int i = 0;
            foreach (var onetweet in postList)
            {
                if (buttonMargin == rbuttonMargin)
                    buttonMargin = lbuttonMargin;
                else
                    buttonMargin = rbuttonMargin;

                Button iButton = new Button();
                iButton.Margin = buttonMargin;
                iButton.Background = backColor;         
                iButton.BorderThickness = borderTh;
                iButton.Click += ShowOneCity;
                iButton.Tag = i;
                i++;

                StackPanel iStackPanel = new StackPanel();

                Image iImage = new Image();
                iImage.Source = new BitmapImage(new Uri(onetweet.cImage, UriKind.Relative));
                iImage.Margin = imageMargin;
                iImage.Stretch = Stretch.Uniform;
                iStackPanel.Children.Add(iImage);

                TextBlock iTextBlock = new TextBlock();
                iTextBlock.Foreground = foregroundColor;
                iTextBlock.Text = onetweet.cName;
                iTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                iTextBlock.Margin = textMargin;
                iTextBlock.Style = (Style)Application.Current.Resources["PhoneTextBlockBase"];
                iStackPanel.Children.Add(iTextBlock);

                iButton.Content = iStackPanel;
                cList.Children.Add(iButton);
            }
        }


        private void abButton1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
           // loaded = false;
            LoadRSS();
        }

        private void ShowOneCity(object sender, RoutedEventArgs e)
        {
            Button iButton = sender as Button;
            RSSFileName = iButton.Tag.ToString() + "rss.xml";
            NavigationService.Navigate(new Uri("/ShowOneCity.xaml?city=" + iButton.Tag.ToString(), UriKind.Relative));
        }

        private void RssList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (RssList.SelectedItem != null)
            {
                NavigationService.Navigate(new Uri("/ShowOnePage.xaml?URL=" + ((PostMessage)(RssList.SelectedItem)).link, UriKind.Relative));
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowOptions.xaml", UriKind.Relative));
        }
    }
}