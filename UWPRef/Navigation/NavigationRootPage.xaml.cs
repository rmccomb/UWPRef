using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPRef.Controls;
using UWPRef.Data;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UWPRef
{
    public sealed partial class NavigationRootPage : Page
    {
        public static NavigationRootPage Current;
        public static Frame RootFrame = null;

        //private RootFrameNavigationHelper _navHelper;
        private PageHeader _header;

        public NavigationView NavigationView
        {
            get { return NavView; }
        }

        public TextBlock AppTitle
        {
            get { return this.appTitle; }
        }

        public NavigationRootPage()
        {
            this.InitializeComponent();
            //_navHelper = new RootFrameNavigationHelper(rootFrame);
            Current = this;
            RootFrame = this.rootFrame;

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        public PageHeader PageHeader
        {
            get
            {
                return _header ?? (_header = UIHelper.GetDescendantsOfType<PageHeader>(NavView).FirstOrDefault());
            }
        }

        #region BackRequested Handlers

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            bool handled = e.Handled;
            this.BackRequested(ref handled);
            e.Handled = handled;
        }

        private void BackRequested(ref bool handled)
        {
            // Get a hold of the current frame so that we can inspect the app back stack.

            if (RootFrame == null)
                return;

            // Check to see if this is the top-most page on the app back stack.
            if (RootFrame.CanGoBack && !handled)
            {
                // If not, set the event to handled and go back to the previous page in the app.
                handled = true;
                RootFrame.GoBack();
            }
        }

        #endregion

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            AddNavigationMenuItems();
        }

        private void AddNavigationMenuItems()
        {
            foreach (var navInfo in NavigationInfoDataSource.Instance.Items)
            {
                var item = new NavigationViewItem() { Content = navInfo.Title, Tag = navInfo.UniqueId, DataContext = navInfo };
                AutomationProperties.SetName(item, navInfo.Title);
                if (navInfo.ImagePath.ToLowerInvariant().EndsWith(".png"))
                {
                    item.Icon = new BitmapIcon() { UriSource = new Uri(navInfo.ImagePath, UriKind.RelativeOrAbsolute) };
                }
                else 
                {
                    item.Icon = new FontIcon()
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = navInfo.ImagePath
                    };
                }
                NavView.MenuItems.Add(item);
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked)
            {
                rootFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                // find NavigationViewItem with Content that equals InvokedItem
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);

            }
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                rootFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                NavView_Navigate(item);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    rootFrame.Navigate(typeof(HomePage));
                    break;

                case "main":
                    rootFrame.Navigate(typeof(MainPage));
                    break;

                case "item1":
                    rootFrame.Navigate(typeof(ItemPage));
                    break;
            }
        }

        private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            EnsureAppTitle();
        }

        private void NavView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EnsureAppTitle();
        }

        private void NavView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnsureAppTitle();
        }

        public bool Extend { get; set; }

        public void EnsureAppTitle()
        {
            if (Extend)
            {
                if (this.NavView.IsPaneOpen)
                    this.appTitle.Visibility = Visibility.Visible;
                else
                    this.appTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.appTitle.Visibility = Visibility.Collapsed;
            }
        }
    }
}

