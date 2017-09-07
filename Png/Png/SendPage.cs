using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Xamarin.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Png
{
    class SendPage : ContentPage
    {
        ResultImg resImg = new ResultImg();       
        Image img = new Image();
        ResponseClass response = new ResponseClass();
        
        public SendPage()
        {
            Title = "Отправить изображение";

            StackLayout stackLayout = new StackLayout();

            Button Choose = new Button
            {
                Text = "Выбрать из имеющихся",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Choose.Clicked += ChooseClicked;
            stackLayout.Children.Add(Choose);

            Button Photo = new Button
            {
                Text = "Создать изображение",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Photo.Clicked += PhotoClicked;
            stackLayout.Children.Add(Photo);

            Frame frame = new Frame
            {
                OutlineColor = Color.Accent
            };

            frame.Content = img;
            Padding = new Thickness(15);

            stackLayout.Children.Add(frame);

            Button Send = new Button
            {
                Text = "Отправить",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Send.Clicked += SendClicked;
            stackLayout.Children.Add(Send);

            ScrollView scrollView = new ScrollView();
            scrollView.Content = stackLayout;
            this.Content = scrollView;
        }

        private async void ChooseClicked(object sender, System.EventArgs e)
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var file = await CrossMedia.Current.PickPhotoAsync( new PickMediaOptions
                {
                    CompressionQuality = 92,
                    PhotoSize = PhotoSize.Small
                });
                img.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });

                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    file.Dispose();
                    resImg.ImgAsByte = memoryStream.ToArray();
                }
            }
        }

        private async void PhotoClicked(object sender, System.EventArgs e)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Png",
                    Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.png",
                    CompressionQuality = 92,
                    PhotoSize = PhotoSize.Small
                });
                if (file == null)
                    return;

                img.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });

                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    file.Dispose();
                    resImg.ImgAsByte = memoryStream.ToArray();
                }
            }
        }

        private async void SendClicked(object sender, System.EventArgs e)
        {
            if (img.Source == null)
            {
                await DisplayAlert("Уведомление", "Пожалуйста выберите/создайте изображение", "ОК");
            }
            else
            {
                bool res = await DisplayAlert("Подтвердить действие", "Отправить изображение?", "Да", "Нет");
                if (res)
                {
                    response.Form = new MultipartFormDataContent();
                   
                    response.Form.Add(new ByteArrayContent(new MemoryStream(resImg.ImgAsByte).ToArray()), "img", "upload.png");

                    response.Client = new HttpClient();

                    response.Uri = new Uri("http://192.168.0.13:52601/api/image/");

                    response.Response = new HttpResponseMessage();
                                     
                    response.Response = await response.Client.PostAsync(response.Uri, response.Form);

                    if (response.Response.StatusCode == HttpStatusCode.OK)
                    {
                        await DisplayAlert("Уведомление", "Изображение успешно отправлено", "ОК");
                    }
                    else
                    {
                        await DisplayAlert("Уведомление", "Не удалось отправить изображение", "ОК");
                    }

                }

            }

        }

        public class ResultImg
        {
            public byte[] ImgAsByte { get; set; }
        }

        public class ResponseClass
        {
            public MultipartFormDataContent Form { get; set; }       
            public HttpClient Client { get; set; }
            public Uri Uri { get; set; }
            public HttpResponseMessage Response { get; set; }

        }
    }
}
