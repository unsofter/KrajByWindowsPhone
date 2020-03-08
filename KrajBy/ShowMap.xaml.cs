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
    public partial class ShowMap : PhoneApplicationPage
    {
        public ShowMap()
        {
            InitializeComponent();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            oneMap.Layers.Add(new Microsoft.Phone.Maps.Controls.MapLayer());
        }
    }
}