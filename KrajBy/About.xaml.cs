using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace KrajBy
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            TiltEffect.SetIsTiltEnabled(this, true);
        }

        public string GetText()
        {
            string txt = "Портал Kraj.by (Край.бай) – белорусский информационно-новостной портал, ежедневно сообщающий о новостях 12 городов:\r\t\r\t" +
                         "Молодечно, Вилейка, Мядель, Воложин, Логойск (Минская область),\r\t\r\t" +
                         "Сморгонь, Островец, Ошмяны (Гродненская область),\r\t\r\t" +
                         "Глубокое, Докшицы, Браслав, Поставы (Витебская область).\r\t\r\t" + 
                         "Основатели сайта ставят себе целью предоставлять объективную, многостороннюю информацию о жизни вышеописанных местностей," +
                         " не применяя оценки, работая в стиле информационных агентств." +
                         " Материалы, размещенные на сайте, являются либо собственными, либо взятыми из других СМИ.\r\t\r\t" + 
                         "Информация о погоде предоставлена сервисом weather.yahoo.com";

            return txt;
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            AboutText.Text = GetText();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}