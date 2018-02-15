using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPRef.Data;
using Windows.Devices.Input;
using Windows.Gaming.Input;
using Windows.System.Profile;
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
        }

        public PageHeader PageHeader
        {
            get
            {
                return _header ?? (_header = UIHelper.GetDescendantsOfType<PageHeader>(NavView).FirstOrDefault());
            }
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

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            //NavView.MenuItems.Add(new NavigationViewItemSeparator());
            NavView.MenuItems.Add(new NavigationViewItem()
            { Content = "My content", Icon = new SymbolIcon(Symbol.Add), Tag = "content" });

            //// set the initial SelectedItem 
            //foreach (NavigationViewItemBase item in NavView.MenuItems)
            //{
            //    if (item is NavigationViewItem && item.Tag.ToString() == "apps")
            //    {
            //        NavView.SelectedItem = item;
            //        break;
            //    }
            //}

            AddNavigationMenuItems();

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

                case "apps":
                    rootFrame.Navigate(typeof(ItemPage));
                    break;

                case "games":
                    rootFrame.Navigate(typeof(ItemPage));
                    break;

                case "music":
                    rootFrame.Navigate(typeof(ItemPage));
                    break;
                case "content":
                    rootFrame.Navigate(typeof(ItemPage));
                    break;
            }
        }
    }
}

