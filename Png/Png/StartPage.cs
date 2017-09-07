using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Png
{
    class StartPage : ContentPage
    {
        Connection connect = new Connection();
        public StartPage()
        {
            Title = "Png";

            StackLayout stackLayout = new StackLayout();

            Button SendButt = new Button
            {
                Text = "Отправить изображение",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            SendButt.Clicked += SendButtClicked;
            stackLayout.Children.Add(SendButt);

            Button GetButt = new Button
            {
                Text = "Принять изображение",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            GetButt.Clicked += GetButtClicked;
            stackLayout.Children.Add(GetButt);

            connect.Stats = new Label
            {
                Text = "Подключение отсутствует",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            stackLayout.Children.Add(connect.Stats);

            connect.Details = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            stackLayout.Children.Add(connect.Details);

            CrossConnectivity.Current.ConnectivityChanged += ConnectChanged;

            ScrollView scrollView = new ScrollView();
            scrollView.Content = stackLayout;
            this.Content = scrollView;
        }

        private async void ConnectChanged(object sender, ConnectivityChangedEventArgs e)
        {
            CheckConnection();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckConnection();
        }

        private void CheckConnection()
        {
            connect.Stats.IsVisible = !CrossConnectivity.Current.IsConnected;
            connect.Details.IsVisible = CrossConnectivity.Current.IsConnected;

            if (CrossConnectivity.Current != null &&
                CrossConnectivity.Current.ConnectionTypes != null &&
                CrossConnectivity.Current.IsConnected == true)
            {
                var Type = CrossConnectivity.Current.ConnectionTypes.FirstOrDefault();
                connect.Details.Text = Type.ToString();
            }
        }

        private async void SendButtClicked(object sender, System.EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await Navigation.PushAsync(new SendPage());
            }
            else
            {
                await DisplayAlert("Уведомление", "Для отправки изображения необходимо подключение к сети", "ОК");
            }
        }

        private async void GetButtClicked(object sender, System.EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await Navigation.PushAsync(new GetPage());
            }
            else
            {
                await DisplayAlert("Уведомление", "Для получения изображения необходимо подключение к сети", "ОК");
            }
        }
    }

    public class Connection
    {
        public Label Stats;

        public Label Details;
    }
}
