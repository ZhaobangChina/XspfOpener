using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XspfOpener.Classes;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace XspfOpener.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlaylistPage : Page
    {
        Xspf xspf;
        StorageFile file;

        public PlaylistPage()
        {
            this.InitializeComponent();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (xspf == null)
                try
                {
                    file = e.Parameter as StorageFile;
                    using (Stream stream = await file.OpenStreamForReadAsync())
                        xspf = Xspf.Load(stream);
                    displayList.DataContext = xspf.TrackList;

                    var mru = StorageApplicationPermissions.MostRecentlyUsedList;
                    mru.Add(file);
                }
                catch
                {
                    MessageDialog msgDialog = new MessageDialog("打开文件失败");
                }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (displayList.SelectedItem != null)
            {
                displayList.SelectedItem = null;
                e.Cancel = true;
            }
            base.OnNavigatingFrom(e);
        }

        string currentFilterText = "";

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newFilterText = (sender as TextBox).Text.ToUpper();
            if (currentFilterText == newFilterText)
                return;
            if (string.IsNullOrEmpty(newFilterText))
            {
                displayList.DataContext = xspf?.TrackList;
            }
            else
            {
                displayList.DataContext = xspf?.TrackList?.
                    Where(x => x.Title.ToUpper().Contains(newFilterText));
            }
            currentFilterText = newFilterText;
        }

        private async void vlcOpenButton_Click(object sender, RoutedEventArgs e)
        {
            XspfTrack trk = (sender as Button).DataContext as XspfTrack;
            if (trk != null)
            {
                string streamUriStr = Uri.EscapeDataString(trk.Location.ToString());
                Uri vlcUri = new Uri(string.Format("vlc://openstream?from=url&url={0}", streamUriStr));
                await Launcher.LaunchUriAsync(vlcUri);
            }
        }

        private async void openButton_Click(object sender, RoutedEventArgs e)
        {
            XspfTrack trk = (sender as Button).DataContext as XspfTrack;
            if(trk!=null)
            {
                await Launcher.LaunchUriAsync(trk.Location);
            }
        }

        private async void openWithButton_Click(object sender, RoutedEventArgs e)
        {
            XspfTrack trk = (sender as Button).DataContext as XspfTrack;
            if(trk!=null)
            {
                LauncherOptions options = new LauncherOptions();
                options.DisplayApplicationPicker = true;
                await Launcher.LaunchUriAsync(trk.Location, options);
            }
        }
    }
}
