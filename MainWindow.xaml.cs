using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NecroVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapPolyline _polyline;
        private Pushpin _pushpin;

        private class LatLng
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        private LatLng GetLocation()
        {
            double lat = 0;
            double lng = 0;

            try
            {
                var result = System.IO.File.ReadAllText(ConfigurationManager.AppSettings["fileLocation"]).Split(new[] { ':' });
                lat = Convert.ToDouble(result[0]);
                lng = Convert.ToDouble(result[1]);

            }
            catch (Exception e)
            {

            }

            return new LatLng()
            {
                Lat = lat,
                Lng = lng,
            };
        }

        public MainWindow()
        {
            InitializeComponent();

            // Center map on current location.

            var location = GetLocation();
            map.Center = new Microsoft.Maps.MapControl.WPF.Location() { Latitude = location.Lat, Longitude = location.Lng, Altitude = 50 };
            map.ZoomLevel = 16;

            // Create timer to poll current location.

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            // Create the polyline.

            _polyline = new MapPolyline()
            {
                Stroke = new System.Windows.Media.LinearGradientBrush(new GradientStopCollection
                                     {
                                       new GradientStop(Color.FromArgb(255, 0, 0, 0), 0.0),
                                       new GradientStop(Color.FromArgb(255, 255, 0, 0), 0.75)
                                     }),
                //SolidColorBrush(System.Windows.Media.Colors.Blue),
                StrokeThickness = 3,
                Opacity = 0.7,
            };
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var location = GetLocation();

            if (location.Lat == 0 || location.Lng == 0)
                return;

            if (_polyline.Locations != null && _polyline.Locations.Count > 0)
            {
                if (_polyline.Locations.Last().Latitude != location.Lat && _polyline.Locations.Last().Longitude != location.Lng)
                {
                    _polyline.Locations.Add(new Location() { Latitude = location.Lat, Longitude = location.Lng });

                    // Update current location pushpin.

                    _pushpin.Location = new Location() { Latitude = location.Lat, Longitude = location.Lng };
                }
            }
            else
            {
                // Create the polyline.

                _polyline.Locations = new LocationCollection()
                {
                    new Location(location.Lat, location.Lng),
                };

                map.Children.Add(_polyline);

                // Create the starting point pushpin. 

                Pushpin pin = new Pushpin()
                {
                    Location = new Location() { Latitude = location.Lat, Longitude = location.Lng },
                    Content = "Begin",
                };

                _pushpin = new Pushpin()
                {
                    Location = new Location() { Latitude = location.Lat, Longitude = location.Lng },
                    Content = "Current",
                };

                map.Children.Add(pin);
                map.Children.Add(_pushpin);
            }
        }
    }
}
