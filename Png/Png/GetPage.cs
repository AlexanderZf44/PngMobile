using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace Png
{
    class GetPage : ContentPage
    {
        Request request = new Request();
        Image img = new Image();

        public GetPage()
        {
            Title = "Принять изображение";

            StackLayout stackLayout = new StackLayout();

            Button Accept = new Button
            {
                Text = "Принять",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Accept.Clicked += AcceptClicked;
            stackLayout.Children.Add(Accept);

            Frame frame = new Frame
            {
                OutlineColor = Color.Accent
            };

            frame.Content = img;
            Padding = new Thickness(15);

            stackLayout.Children.Add(frame);

            ScrollView scrollView = new ScrollView();
            scrollView.Content = stackLayout;
            this.Content = scrollView;
        }

        private async void AcceptClicked(object sender, System.EventArgs e)
        {
            request.Client = new HttpClient();

            request.Uri = new Uri("http://192.168.0.13:52601/api/image/");

            request.Result = await request.Client.GetByteArrayAsync(request.Uri);

            img.Source = ImageSource.FromStream(() => new MemoryStream(request.Result));
        }

        public class Request
        {
            public HttpClient Client { get; set; }
            public Uri Uri { get; set; }
            public byte[] Result { get; set; }
        }
    }
}
