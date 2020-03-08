using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace KrajBy
{
    public partial class ShowOptions : PhoneApplicationPage
    {
        int selCity = 0;
        Functions allFunc = new Functions();

        public ShowOptions()
        {
            InitializeComponent();
            TiltEffect.SetIsTiltEnabled(this, true);
        }

        private void RB_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Braslav.IsChecked == true)
                selCity = 0;
            else if (Vileika.IsChecked == true)
                selCity = 1;
            else if (Vologin.IsChecked == true)
                selCity = 2;
            else if (Glubokoe.IsChecked == true)
                selCity = 3;
            else if (Dokshitsi.IsChecked == true)
                selCity = 4;
            else if (Logoisk.IsChecked == true)
                selCity = 5;
            else if (Molodecho.IsChecked == true)
                selCity = 6;
            else if (Myadel.IsChecked == true)
                selCity = 7;
            else if (Ostrovets.IsChecked == true)
                selCity = 8;
            else if (Oshmyani.IsChecked == true)
                selCity = 9;
            else if (Postavi.IsChecked == true)
                selCity = 10;
            else if (Smorhon.IsChecked == true)
                selCity = 11;
            else
                selCity = 0;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            selCity = allFunc.wCity;
            switch (selCity)
            {
                case 0:
                    Braslav.IsChecked = true;
                    break;
                case 1:
                    Vileika.IsChecked = true;
                    break;
                case 2:
                    Vologin.IsChecked = true;
                    break;
                case 3:
                    Glubokoe.IsChecked = true;
                    break;
                case 4:
                    Dokshitsi.IsChecked = true;
                    break;
                case 5:
                    Logoisk.IsChecked = true;
                    break;
                case 6:
                    Molodecho.IsChecked = true;
                    break;
                case 7:
                    Myadel.IsChecked = true;
                    break;
                case 8:
                    Ostrovets.IsChecked = true;
                    break;
                case 9:
                    Oshmyani.IsChecked = true;
                    break;
                case 10:
                    Postavi.IsChecked = true;
                    break;
                case 11:
                    Smorhon.IsChecked = true;
                    break;
            }
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            allFunc.wCity = selCity;
        }
    }
}