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
using OValiantHearts.ViewModels;

namespace OValiantHearts
{
    public partial class Feed : PhoneApplicationPage
    {
        private FeedItemViewModel feed;

        public Feed()
        {
            InitializeComponent();

            feed = new FeedItemViewModel();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            feed.Username = this.NavigationContext.QueryString["username"];
            feed.Message = this.NavigationContext.QueryString["message"];
            feed.Date = this.NavigationContext.QueryString["date"];
            feed.Thumbnail = this.NavigationContext.QueryString["thumbnail"];
            feed.Link = this.NavigationContext.QueryString["link"];

            DataContext = feed;
        }
    }
}