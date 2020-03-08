using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;

namespace Phone.Controls
{
    public class ProgressIndicator : ContentControl
    {       
        private System.Windows.Shapes.Rectangle backgroundRect;
        private System.Windows.Controls.StackPanel stackPanel;
        private System.Windows.Controls.ProgressBar progressBar;
        private System.Windows.Controls.TextBlock textBlockStatus;

        private ProgressTypes progressType;
        private bool currentSystemTrayState;       
        private static string defaultText = "Загрузка...";
        private bool showLabel;
        private string labelText;
        
        
        public ProgressIndicator()
        {
            this.DefaultStyleKey = typeof(ProgressIndicator);          
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.backgroundRect = this.GetTemplateChild("backgroundRect") as Rectangle;
            this.stackPanel = this.GetTemplateChild("stackPanel") as StackPanel;
            this.progressBar = this.GetTemplateChild("progressBar") as ProgressBar;
            this.textBlockStatus = this.GetTemplateChild("textBlockStatus") as TextBlock;

            this.Text = labelText;
            
            InitializeProgressType();
        }

        internal Popup ChildWindowPopup
        {
            get;
            private set;
        }

        private static PhoneApplicationFrame RootVisual
        {
            get
            {
                return Application.Current == null ? null : Application.Current.RootVisual as PhoneApplicationFrame;
            }
        }


        public ProgressTypes ProgressType
        {
            get
            {
                return this.progressType;
            }
            set
            {
                progressType = value;
            }
        }

        public bool ShowLabel
        {
            get
            {
                return this.showLabel;
            }
            set
            {
                this.showLabel = value;
            }
        }

        public string Text
        {
            get
            {
                return labelText;                
            }
            set
            {
                this.labelText = value;
                if (this.textBlockStatus != null)
                {
                    this.textBlockStatus.Text = value;
                }
            }
        }

        public ProgressBar ProgressBar
        {
            get
            {
                return this.progressBar;
            }
        }

        public new double Opacity
        {
            get
            {
                return this.backgroundRect.Opacity;
            }
            set
            {
                this.backgroundRect.Opacity = value;
            }
        }

        public void Hide()
        {
            // Restore system tray
            SystemTray.IsVisible = currentSystemTrayState;
            this.progressBar.IsIndeterminate = false;
            this.ChildWindowPopup.IsOpen = false;

        }

        public void Show()
        {
            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = new Popup();

                try
                {
                    this.ChildWindowPopup.Child = this;
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException("The control is already shown.");
                }
            }


            if (this.ChildWindowPopup != null && Application.Current.RootVisual != null)
            {
                // Configure accordingly to the type
                InitializeProgressType();

                // Show popup
                this.ChildWindowPopup.IsOpen = true;
            }
        }
      

        private void HideSystemTray()
        {
            // Capture current state of the system tray
            this.currentSystemTrayState = SystemTray.IsVisible;
            // Hide it
            SystemTray.IsVisible = false;
        }

        private void InitializeProgressType()
        {
            this.HideSystemTray();
            if (this.progressBar == null)
                return;

            this.progressBar.Value = 0;


            switch (this.progressType)
            {
                case ProgressTypes.WaitCursor:
                    this.Opacity = 0.7;
                    this.backgroundRect.Visibility = System.Windows.Visibility.Visible;
                    this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                    this.textBlockStatus.Text = defaultText;
                    this.textBlockStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                    this.textBlockStatus.Margin = new Thickness();
                    this.Height = 800;
                    this.progressBar.IsIndeterminate = true;
                    break;
                case ProgressTypes.DeterminateMiddle:
                    this.Opacity = 0.7;
                    this.backgroundRect.Visibility = System.Windows.Visibility.Visible;
                    this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                    if (showLabel)
                    {
                        this.textBlockStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                        this.textBlockStatus.Margin = new Thickness();
                    }
                    else
                    {
                        this.textBlockStatus.Margin = new Thickness();
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    this.Height = 800;
                    break;
                case ProgressTypes.DeterminateTop:
                    this.Opacity = 0.8;
                    this.backgroundRect.Visibility = System.Windows.Visibility.Visible;
                    this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                        this.textBlockStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        this.textBlockStatus.Margin = new Thickness(18, -5, 0, 0);
                        this.Height = 30;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                        this.Height = 4;
                    }

                    break;
                case ProgressTypes.IndeterminateTop:
                    this.Opacity = 0.8;
                    this.backgroundRect.Visibility = System.Windows.Visibility.Visible;
                    this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    this.progressBar.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                        this.textBlockStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        this.textBlockStatus.Margin = new Thickness(18, -5, 0, 0);
                        this.Height = 30;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                        this.Height = 4;
                    }
                    this.progressBar.IsIndeterminate = true;
                    break;
                case ProgressTypes.CustomTop:
                    this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                        this.textBlockStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        this.textBlockStatus.Margin = new Thickness(18, -5, 0, 0);
                        this.Height = 30;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                        this.Height = 4;
                    }
                    break;
                case ProgressTypes.CustomMiddle:
                    this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    if (showLabel)
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.textBlockStatus.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    this.Height = 800;
                    break;
            }
        }

    }
}
