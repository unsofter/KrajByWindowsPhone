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
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net.NetworkInformation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace KrajBy
{
    public partial class ShowOneCity : PhoneApplicationPage
    {
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

        // Общая лента
        string[] RSSs = { 
                            "http://kraj.by/braslav/news/",    // Браслав     0
                            "http://kraj.by/vilejka/news/",    // Вилейка     1
                            "http://kraj.by/volozhin/news/",   // Воложин     2
                            "http://kraj.by/glubokoe/news/",   // Глубокое    3
                            "http://kraj.by/dokshitcy/news/",  // Докшицы     4
                            "http://kraj.by/logojsk/news/",    // Логойск     5
                            "http://kraj.by/molodechno/news/", // Молодечно   6
                            "http://kraj.by/mjadel/news/",     // Мядель      7
                            "http://kraj.by/ostrovets/news/",  // Островец    8
                            "http://kraj.by/oshmjany/news/",   // Ошмяны      9
                            "http://kraj.by/postavy/news/",    // Поставы     10
                            "http://kraj.by/smorgon/news/",    // Сморгонь    11
                        };

        // Названия новостей
        string[] NewsNames = { 
                                    "Новости",      // 0
                                    "События",      // 1
                                    "Экономика",    // 2
                                    "Происшествия", // 3
                                    "Культура",     // 4
                                    "Спорт"         // 5
                        };
        string[] NewsNamesRSS = {
                                    "?rss=articles",   // Новости
                                    "sobitiya/",       // События
                                    "ekonomika/",      // Экономика
                                    "proisshestviya/", // Проишествия
                                    "kultura/",        // Культура
                                    "sport/"           // Спорт
                                };
        int NewsNamesIndex;
        const int MaxNewsNamesIndex = 5;

        // RSS
        const string RSSadd = "?rss=articles";
        // Рстановка по кнопке "Назад"
        bool stoped = false;

        // Текущий город
        int curCity;

        /// <summary>
        ///  Уже было загружено
        /// </summary>
        bool loaded = false;

        Functions allFunc = new Functions();

        progessOnFront progressOn;

        public ShowOneCity()
        {
            InitializeComponent();
            TiltEffect.SetIsTiltEnabled(this, true);
        }

        void LoadOneCity()
        {
            if (loaded) return;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Отсутствует соединение с сетью интернет.");
                return;
            }

            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn.IsEnabled = false;

            if (progressOn == null)
                progressOn = new progessOnFront();

            NewsNamesIndex = 0;
            progressOn.Show(this);
            
            OneCity.Items.Clear();
            // Основные новости
            string RSS = RSSs[curCity] + RSSadd;
            OneCity.Title = "Портал kraj.by. Новости в городах края. " + CityNames[curCity] + ".";
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(MainDownload);
            client.DownloadStringAsync(new Uri(RSS));
            loaded = true;
        }

        void MainDownload(object sender, DownloadStringCompletedEventArgs e)
        {
            string RSSString;
            if (e.Error == null)
            {
                RSSString = e.Result;
                ParseRSSAndBindData(RSSString, NewsNames[NewsNamesIndex]);
            }
            else
                MessageBox.Show("Нет доступа к сайту. Попробуете повторить попытку позже.");

            NewsNamesIndex++;

            if (NewsNamesIndex > MaxNewsNamesIndex)
            {
                ApplicationBarIconButton btn =  (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn.IsEnabled = true;
                OneCity_SelectionChanged(null, null);
                progressOn.Hide();
                return;
            }

            if (stoped)
                return;

            string RSS = RSSs[curCity] + NewsNamesRSS[NewsNamesIndex] + RSSadd;
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(MainDownload);
            client.DownloadStringAsync(new Uri(RSS));
        }

        void ParseRSSAndBindData(string RSSString, string Title)
        {
            try
            {
                XElement iNews = XElement.Parse(RSSString);

                var postList =
                    from tweet in iNews.Descendants("item")
                    select new PostMessage
                    {
                        title = tweet.Element("title").Value,
                        mainImage = allFunc.GetImageFromPostContents(tweet.Element("description").Value),
                        description = Regex.Replace(tweet.Element("description").Value, "<.*?>", String.Empty),
                        pubDate = Convert.ToDateTime(tweet.Element("pubDate").Value).ToString(),
                        link = tweet.Element("link").Value
                    };

                PanoramaItem iPanoramaItem = new PanoramaItem();
                iPanoramaItem.Header = Title;
                iPanoramaItem.Orientation = System.Windows.Controls.Orientation.Horizontal;
                ScrollViewer iScrollViewer = new ScrollViewer();

                StackPanel mStackPanel = new StackPanel();
                mStackPanel.Margin = new Thickness(0, 4, 16, 0);
                mStackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                mStackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                SolidColorBrush backColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xDF, 0xD6, 0xAB));
                SolidColorBrush foreColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x3A, 0x3A));
                StackPanel firstStackPanel = new StackPanel();
                StackPanel secondStackPanel = new StackPanel();
                StackPanel thirdStackPanel = new StackPanel();

                firstStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                secondStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                secondStackPanel.Margin = new Thickness(0, 12, 0, 0);
                thirdStackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                thirdStackPanel.Margin = new Thickness(0, 12, 0, 0);

                int i = 0;
                foreach (var iNew in postList)
                {
                    HubTile iHubTile = new HubTile();
                    iHubTile.Margin = new Thickness(12, 0, 0, 0);
                    iHubTile.Title = iNew.pubDate.ToString(); // Title;
                    iHubTile.Message = iNew.title;
                    iHubTile.Notification = iNew.title;
                    iHubTile.Source = new BitmapImage(new Uri(iNew.mainImage, UriKind.Absolute));
                    iHubTile.Background = backColor;
                    iHubTile.Foreground = foreColor;
                    iHubTile.Tag = iNew.link;
                    iHubTile.Tap += ShowWebNew;
                    iHubTile.GroupTag = Title;
                    iHubTile.Name = Title + i.ToString();
                    iHubTile.Size = TileSize.Medium;

                    if (i > 6)
                        thirdStackPanel.Children.Add(iHubTile);
                    else if (i > 3)
                        secondStackPanel.Children.Add(iHubTile);
                    else
                    {
                        HubTileService.FreezeHubTile(iHubTile);
                        firstStackPanel.Children.Add(iHubTile);
                    }
                    i++;
                }
                mStackPanel.Children.Add(firstStackPanel);
                mStackPanel.Children.Add(secondStackPanel);
                mStackPanel.Children.Add(thirdStackPanel);
                iScrollViewer.Content = mStackPanel;
                iPanoramaItem.Content = iScrollViewer;
                OneCity.Items.Add(iPanoramaItem);
            }
            catch
            {
            }
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            string cCity;
            if (NavigationContext.QueryString.TryGetValue("city", out cCity))
                curCity = Convert.ToInt16(cCity);
                        
            LoadOneCity();
        }

        private void ShowWebNew(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HubTile iHubTile = sender as HubTile;
  //          WebBrowserTask webTask = new WebBrowserTask();
          //  webTask.Uri = new Uri(iHubTile.Tag.ToString());
//            webTask.Show();
            NavigationService.Navigate(new Uri("/ShowOnePage.xaml?URL=" + iHubTile.Tag.ToString(), UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            loaded = false;
            LoadOneCity();
        }

        private void OneCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            for (int i = 0; i <= MaxNewsNamesIndex; i ++)
                if (i == OneCity.SelectedIndex)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        HubTile iHubtile = (HubTile)this.FindName(NewsNames[i] + j.ToString());
                        VisualStateManager.GoToState(iHubtile, "Flipped", true);
                    }

//                    HubTileService.UnfreezeGroup(NewsNames[i]);
                }
 //               else
   //                 HubTileService.FreezeGroup(NewsNames[i]);
        }

        private void PhoneApplicationPage_BackKeyPress_1(object sender, CancelEventArgs e)
        {
            stoped = true;

            if(progressOn != null)
                progressOn.Hide();
        }
    }
}