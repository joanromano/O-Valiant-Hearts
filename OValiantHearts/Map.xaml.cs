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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using OValiantHearts.ViewModels;
using System.Globalization;

namespace OValiantHearts
{
    public partial class Map : PhoneApplicationPage
    {
        private ConcertItemViewModel concert;

        //Guardaremos aqui el Rect de localización para poder centrar el mapa
        IEnumerable<GeoCoordinate> loc = null;

        public Map()
        {
            InitializeComponent();
            concert = new ConcertItemViewModel();
        }

        //Se ejecuta al cargar la página
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            concert.Title = this.NavigationContext.QueryString["title"];
            concert.Date = this.NavigationContext.QueryString["date"];
            concert.Latitude = this.NavigationContext.QueryString["lat"];
            concert.Longitude = this.NavigationContext.QueryString["long"];
            
            drawConcertToMap();
        }

        void drawConcertToMap()
        {
            
            map1.Children.Clear();

            //map1.Mode = new AerialMode();

            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate(Double.Parse(concert.Latitude, CultureInfo.InvariantCulture), Double.Parse(concert.Longitude, CultureInfo.InvariantCulture));
            pin.MouseLeftButtonUp += new MouseButtonEventHandler(pinMouseLeftButtonUp);
            pin.Template = (ControlTemplate)this.Resources["PinTemplate"];
            pin.Name = concert.Title;
            map1.Children.Add(pin);

            //Se hace un setView para encuadrar el Mapa. Para esto, se ponen todos los puntos en un IEnumerable y el sistema hace el encuadre solo
            List<GeoCoordinate> l = new List<GeoCoordinate>();
            foreach (Pushpin p in map1.Children)
            {
                l.Add(p.Location);
            }

            loc = l;
            map1.SetView(LocationRect.CreateLocationRect(loc));

            map1.ZoomLevel = 15;
        }

        //Evento al pinchar sobre el pin de un concierto
        void pinMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(concert.Title + " - " + concert.Date);
        }
    }
}