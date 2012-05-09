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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.Windows.Navigation;
using MyToolkit.Multimedia;
using MyToolkit.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OValiantHearts.ViewModels;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace OValiantHearts
{
    public partial class MainPage : PhoneApplicationPage
    {
       // PanoramaViewModel panoramaViewModel;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            //DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

           // panoramaViewModel = new PanoramaViewModel();

            // Set the data context of the listbox control to the sample data and load sources
            DataContext = new PanoramaViewModel();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                //App.ViewModel.LoadData();
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (YouTube.CancelPlay()) // used to abort current youtube download
                e.Cancel = true;
            else
            {
                // your code here
            }
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            YouTube.CancelPlay(); // used to reenable page
            // your code here
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //YouTube.Play("rW5j3z0AMhk", true, YouTubeQuality.Quality480P);
        }

        private void VideosList_Tap(object sender, GestureEventArgs e)
        {
            // if an item is selected
            if (VideosList.SelectedIndex != -1)
            {
                YouTube.Play(((VideoItemViewModel)VideosList.SelectedItem).Id, true, YouTubeQuality.Quality480P);
            }
        }

        private void ConcertsList_Tap(object sender, GestureEventArgs e)
        {
            // if an item is selected
            if (ConcertsList.SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/Map.xaml?title=" + ((ConcertItemViewModel)ConcertsList.SelectedItem).Title
                                                                        + "&date=" + ((ConcertItemViewModel)ConcertsList.SelectedItem).Date
                                                                        + "&lat=" + ((ConcertItemViewModel)ConcertsList.SelectedItem).Latitude
                                                                        + "&long=" + ((ConcertItemViewModel)ConcertsList.SelectedItem).Longitude, UriKind.Relative));
            }
        }

        private void FeedsList_Tap(object sender, GestureEventArgs e)
        {
            // if an item is selected
            if (FeedsList.SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/Feed.xaml?username=" + ((FeedItemViewModel)FeedsList.SelectedItem).Username
                                                                        + "&date=" + ((FeedItemViewModel)FeedsList.SelectedItem).Date
                                                                        + "&message=" + ((FeedItemViewModel)FeedsList.SelectedItem).Message
                                                                        + "&thumbnail=" + ((FeedItemViewModel)FeedsList.SelectedItem).Thumbnail
                                                                        + "&link=" + ((FeedItemViewModel)FeedsList.SelectedItem).Link,
                                                                        UriKind.Relative));
            }
        }
    }
}