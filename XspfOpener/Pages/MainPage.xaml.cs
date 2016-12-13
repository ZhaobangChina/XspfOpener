using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace XspfOpener.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<StorageFile> recentFileList
            = new ObservableCollection<StorageFile>();

        public MainPage()
        {
            this.InitializeComponent();
            recentFileDisplayList.DataContext = recentFileList;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            recentFileList.Clear();

            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            foreach (var entry in mru.Entries)
            {
                try
                {
                    StorageFile file = await mru.GetFileAsync(entry.Token);
                    recentFileList.Add(file);
                }
                catch { }
            }
            recentFileDisplayList.Visibility = recentFileList.Count == 0 ?
                Visibility.Collapsed : Visibility.Visible;
        }

        private async void openButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".xspf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                recentFileList.Insert(0, file);
                Frame.Navigate(typeof(PlaylistPage), file);
            }
        }

        private void recentFileDisplayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            StorageFile fileClicked = e.ClickedItem as StorageFile;
            if (fileClicked != null)
            {
                Frame.Navigate(typeof(PlaylistPage), fileClicked);
                recentFileList.Remove(fileClicked);
                recentFileList.Insert(0, fileClicked);
            }
        }
    }
}