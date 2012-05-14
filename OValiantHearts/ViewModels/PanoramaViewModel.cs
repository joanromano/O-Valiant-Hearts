using System.Collections.ObjectModel;
using MyToolkit.Networking;
using System.Windows;
using Newtonsoft.Json.Linq;
using DanielVaughan.WindowsPhone7Unleashed;
using System.Threading;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Diagnostics;
using Microsoft.Phone.Net.NetworkInformation;

namespace OValiantHearts.ViewModels
{
    public class PanoramaViewModel : INotifyPropertyChanged

    {
        private DateTime _lastMessageNoConnection = new DateTime(1970, 1, 1);

        /// <summary>
        /// A collection for VideoItemViewModel objects.
        /// </summary>
        readonly ObservableCollection<VideoItemViewModel> videos = new ObservableCollection<VideoItemViewModel>();
        public ObservableCollection<VideoItemViewModel> Videos
        {
            get
            {
                return videos;
            }
        }

        /// <summary>
        /// A collection for FeedItemViewModel objects.
        /// </summary>
        readonly ObservableCollection<FeedItemViewModel> feeds = new ObservableCollection<FeedItemViewModel>();

        public ObservableCollection<FeedItemViewModel> Feeds
        {
            get
            {
                return feeds;
            }
        }

        /// <summary>
        /// A collection for ConcertItemViewModel objects.
        /// </summary>
        readonly ObservableCollection<ConcertItemViewModel> concerts = new ObservableCollection<ConcertItemViewModel>();

        public ObservableCollection<ConcertItemViewModel> Concerts
        {
            get 
            {
                return concerts;
            }
        }

        private String paging;

        public PanoramaViewModel()
        {
            videoBusy = true;
            feedBusy = true;
            tourBusy = true;
            Busy = true;

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                long totalMiliSecond = (DateTime.UtcNow.Ticks - _lastMessageNoConnection.Ticks) / 10000;
                if (totalMiliSecond < 10000) return;

                _lastMessageNoConnection = DateTime.UtcNow;

                MessageBox.Show("You need internet connection to request city information. Please, chack your internet connetion and try again.");


                return;
            }

            loadVideos();
            loadFeeds();
            loadTour();

            paging = "";

            fetchMoreDataCommand = new DelegateCommand(
                obj =>
                {
                    if (busy)
                    {
                        return;
                    }
                    Busy = true;
                    ThreadPool.QueueUserWorkItem(
                        delegate
                        {
                            /* This Thread.Sleep is just to demonstrate a slow operation. */
                            // Thread.Sleep(3000);
                            /* We invoke back to the UI thread. 
                             * Ordinarily this would be done 
                             * by the Calcium infrastructure automatically. */
                            Deployment.Current.Dispatcher.BeginInvoke(
                                delegate
                                {
                                    if (!"".Equals(paging))
                                        loadPaging();
                                    else
                                        Busy = false;
                                });
                        });

                });
        }

        public void loadVideos()
        {
            var request = new HttpGetRequest("http://gdata.youtube.com/feeds/api/users/ovalianthearts/uploads");
            request.Query.Add("v", "2");
            request.Query.Add("alt", "json");

            Http.Get(request, VideoListRequestFinished);
        }

        public void loadFeeds()
        {
            var request = new HttpGetRequest("https://graph.facebook.com/oauth/access_token");
            request.Query.Add("client_id", "270546663027963");
            request.Query.Add("client_secret", "9c7eb6f1a4ddaef888fa73dd0734d896");
            request.Query.Add("grant_type", "client_credentials");

            Http.Get(request, AccesTokenRequestFinished);
        }

        public void loadTour()
        {
            var request = new HttpGetRequest("http://dl.dropbox.com/u/8849803/concerts.json");

            Http.Get(request, TourRequestFinished);
        }

        public void loadPaging()
        {
            var request = new HttpGetRequest(paging);

            Http.Get(request, FeedRequestFinished);
        }

        private void VideoListRequestFinished(HttpResponse response)
        {
            if (response.Successful)
            {
                // process response outside UI thread

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // set your data to view model or control (this code is called in the UI thread)

                    var jsonResponse = response.Response;
                    JObject jsonObject = JObject.Parse(jsonResponse);
                    JArray entries = (JArray)jsonObject["feed"]["entry"];

                    for (int i = 0; i < entries.Count; i++)
                    {
                        VideoItemViewModel videoItem = new VideoItemViewModel();
                        string[] idSplit =  entries[i]["id"]["$t"].ToString().Split(':');
                        videoItem.Id = (idSplit[idSplit.Length-1]);
                        videoItem.Title = entries[i]["title"]["$t"].ToString();
                        videoItem.Thumbnail = ((JArray)entries[i]["media$group"]["media$thumbnail"])[0]["url"].ToString();
                        videoItem.Date = entries[i]["published"]["$t"].ToString();
                        this.Videos.Add(videoItem);
                    }
                    videoBusy = false;
                    updateBusy();
                });
            }
            else
            {
                if (!response.Canceled)
                {
                    videoBusy = false;
                    updateBusy();
                    // display exception
                    //MessageBox.Show(response.Exception.Message);
                }
            }
        }

        private void AccesTokenRequestFinished(HttpResponse response)
        {
            if (response.Successful)
            {
                // process response outside UI thread

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // set your data to view model or control (this code is called in the UI thread)

                    var jsonResponse = response.Response;
                    var request = new HttpGetRequest("https://graph.facebook.com/ovaliantheartsmusic/feed");
                    request.Query.Add("access_token", jsonResponse.Split('=')[1]);

                    Http.Get(request, FeedRequestFinished);

                });
            }
            else
            {
                if (!response.Canceled)
                {
                    // display exception
                    //MessageBox.Show(response.Exception.Message);
                }
            }
        }

        private void FeedRequestFinished(HttpResponse response)
        {
            if (response.Successful)
            {
                // process response outside UI thread

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // set your data to view model or control (this code is called in the UI thread)
                    var jsonResponse = response.Response;
                    JObject jsonObject = JObject.Parse(jsonResponse);
                    JArray entries = (JArray)jsonObject["data"];

                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i]["message"] != null)
                        {
                            FeedItemViewModel feedItem = new FeedItemViewModel();
                            feedItem.Message = entries[i]["message"].ToString();
                            feedItem.Username = entries[i]["from"]["name"].ToString();
                            feedItem.Date = entries[i]["created_time"].ToString();

                            if (entries[i]["picture"] != null)
                            { feedItem.Thumbnail = entries[i]["picture"].ToString(); }
                            if (entries[i]["link"] != null)
                            { feedItem.Link = entries[i]["link"].ToString(); }

                            feeds.Add(feedItem);
                        }
                    }
                    
                    if (jsonObject["paging"] != null)
                    {
                        paging = jsonObject["paging"]["next"].ToString();
                    }
                    else
                    {
                        paging = "";
                    }
                    feedBusy = false;
                    updateBusy();
                });
            }
            else
            {
                if (!response.Canceled)
                {
                    feedBusy = false;
                    updateBusy();
                    // display exception
                    //MessageBox.Show(response.Exception.Message);
                }
            }
        }

        private void TourRequestFinished(HttpResponse response)
        {
            if (response.Successful)
            {
                // process response outside UI thread

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // set your data to view model or control (this code is called in the UI thread)

                    var jsonResponse = response.Response;
                    JObject jsonObject = JObject.Parse(jsonResponse);
                    JArray entries = (JArray)jsonObject["concerts"];

                    for (int i = 0; i < entries.Count; i++)
                    {
                            ConcertItemViewModel concertItem = new ConcertItemViewModel();
                            concertItem.Title = entries[i]["title"].ToString();
                            concertItem.Date = entries[i]["subtitle"].ToString();
                            concertItem.Latitude = entries[i]["latitude"].ToString();
                            concertItem.Longitude = entries[i]["longitude"].ToString();

                            this.Concerts.Add(concertItem);
                    }
                    tourBusy = false;
                    updateBusy();
                });
            }
            else
            {
                if (!response.Canceled)
                {
                    tourBusy = false;
                    updateBusy();
                    // display exception
                    //MessageBox.Show(response.Exception.Message);
                }
            }
        }

        readonly DelegateCommand fetchMoreDataCommand;

        public ICommand FetchMoreDataCommand
        {
            get
            {
                return fetchMoreDataCommand;
            }
        }

        bool busy, videoBusy, feedBusy, tourBusy;

        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                if (busy == value)
                {
                    return;
                }
                busy = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Busy"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var tempEvent = PropertyChanged;
            if (tempEvent != null)
            {
                tempEvent(this, e);
            }
        }

        private void updateBusy()
        {
            if (!videoBusy && !feedBusy && !tourBusy)
                Busy = false;
        }
    }
}
