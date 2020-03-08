using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using HtmlAgilityPack;

namespace KrajBy
{
    public partial class ShowOnePage : PhoneApplicationPage
    {
        String curURL;
        progessOnFront progressOn;

        public ShowOnePage()
        {
            InitializeComponent();
            TiltEffect.SetIsTiltEnabled(this, true);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.TryGetValue("URL", out curURL))
                LoadOnePage();
        }

        private void LoadOnePage()
        {
            if (progressOn == null)
                progressOn = new progessOnFront();

            progressOn.Show(this);

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadPage);
            client.DownloadStringAsync(new Uri(curURL));
        }

        void DownloadPage(object sender, DownloadStringCompletedEventArgs e)
        {
            HtmlDocument html = new HtmlDocument();
            try
            {
                if (e.Error == null)
                    html.LoadHtml(e.Result);
                else
                {
                    MessageBox.Show("Нет доступа к сайту. Попробуете повторить попытку позже.");
                    return;
                }


                foreach (HtmlNode imglnk in html.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string lnkValue = imglnk.GetAttributeValue("href", null);
                    if ((lnkValue.Length > 4) && (lnkValue.Substring(0, 4) == "/img"))
                        imglnk.SetAttributeValue("href", "http://www.kraj.by" + lnkValue);
                    else if ((lnkValue.Length > 0) && (lnkValue.Substring(0, 1) == "/"))
                        imglnk.SetAttributeValue("href", "http://www.kraj.by" + lnkValue);
                }

                foreach (HtmlNode img in html.DocumentNode.SelectNodes("//img[@src]"))
                {
                    string srcValue = img.GetAttributeValue("src", null);
                    if ((srcValue.Length > 4) && (srcValue.Substring(0, 4) == "/img"))
                        img.SetAttributeValue("src", "http://www.kraj.by" + srcValue);
                }

                var res = html.DocumentNode.SelectSingleNode("//div[@class='uc']");

                string HTMLString = "<html><body style='background:#FFF6CB;'><div style='background:#FFF6CB;text-align:justify;'>" + res.InnerHtml + "</div></body></html>";

                pBrowse.NavigateToString(HTMLString);
            }
            finally
            {
                progressOn.Hide();
            }
        }
    }
}