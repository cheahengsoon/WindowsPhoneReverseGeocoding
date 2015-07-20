using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Text;

namespace ReverseGeoCodingWP8
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void GetLocation_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get the current position
                var geolocator = new Geolocator() { DesiredAccuracyInMeters = 10 };

                var geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

                //Perform the reverse geocode query 
                var query = new ReverseGeocodeQuery() { GeoCoordinate = geoposition.Coordinate.ToGeoCoordinate() };
                var geoCodeResults = await query.GetMapLocationsAsync();
                var address = geoCodeResults.First().Information.Address;

                //Print all 
                Coordinates.Text = FormatCoordinates(geoposition);
                Address.Text = FormatAddress(address);

            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    //Location services turned off, ask user to turn it on.
                    AskUserToTurnOnLocationServices();
                }
                else
                {
                    MessageBox.Show("Unable to get location");
                }
            }
        }

        private static async void AskUserToTurnOnLocationServices()
        {
            var result = MessageBox.Show("This app needs to access your location data", "Would you like to turn on Location Services?", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
            }
        }

        private string FormatAddress(MapAddress address)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(address.Street).Append(",")
                .Append(address.HouseNumber).Append("\n")
                .Append(address.City).Append("\n")
                .Append(address.Country).Append("\n")
                .Append(address.Continent);

            return sb.ToString();
        }

        private string FormatCoordinates(Geoposition geoposition)
        {
            return String.Format("Latitude: {0}\nLongitude: {1}", geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
        }
    }
}