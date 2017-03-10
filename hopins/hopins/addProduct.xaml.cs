using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hopins
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class addProduct : Page
    {
        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;

        public addProduct()
        {
            this.InitializeComponent();
            synthesizer = new SpeechSynthesizer();

            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");
        }

        private async void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            if(captureManager == null)
            { 
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();
            }

            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();

            try
            {
                await play("This is add product page");
            }
            catch (System.IO.FileNotFoundException)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Media player components unavailable");
                await messageDialog.ShowAsync();
            }
            catch (Exception)
            {
                media.AutoPlay = false;
                var messageDialog = new Windows.UI.Popups.MessageDialog("Unable to synthesize text");
                await messageDialog.ShowAsync();
            }




        }

        async void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                await recognize();
            }
            catch (Exception)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Unable to synthesize text");
                await messageDialog.ShowAsync();
            }

        }

        async Task recognize()
        {

            if (this.recognizer == null)
            {
                this.recognizer = new SpeechRecognizer();
            }


            await this.recognizer.CompileConstraintsAsync();
            var result = await this.recognizer.RecognizeAsync();

            if (result.Text == "capture")
            {
                Test.Text = result.Text;
                ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

                // create storage file in local app storage
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "TestPhoto.jpg",
                    CreationCollisionOption.GenerateUniqueName);

                // take photo
                await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);

                // Get photo as a BitmapImage
                BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));


                if (i == 0)
                {
                    imagePreivew.Source = bmpImage;
                    i++;
                }


                else if (i == 1)
                {
                    imagePreivew2.Source = bmpImage;
                    i++;
                }

                else if (i == 2)
                {
                    imagePreivew3.Source = bmpImage;
                    i++;
                }

                else
                {
                    imagePreivew.Source = bmpImage;
                    i = 1;
                }

                await play("image captured");

            }
            else if(result.Text=="home"){
                Test.Text = result.Text;
                this.Frame.Navigate(typeof(MainPage), null);
            }

            else
            {
                Test.Text = result.Text;
                await play("i dont understand");

            }

        }


        async Task play(string welcome)
        {
            SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(welcome);
            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
        }


        Windows.Media.Capture.MediaCapture captureManager;

        async private void InitCamera_Click(object sender, RoutedEventArgs e)
        {
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();
        }

        async private void StartCapturePreview_Click(object sender, RoutedEventArgs e)
        {

            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();
        }

        //async private void StopCapturePreview_Click(object sender, RoutedEventArgs e)
        //{
        //    await captureManager.StopPreviewAsync();
        //}

        int i = 0;

        async private void CapturePhoto_Click(object sender, RoutedEventArgs e)
        {
           

            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.GenerateUniqueName);

            // take photo
            await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);

            // Get photo as a BitmapImage
            BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));


            if (i == 0)
            {
                imagePreivew.Source = bmpImage;
                i++;
            }


            else if (i == 1)
            {
                imagePreivew2.Source = bmpImage;
                i++;
            }

            else if (i == 2)
            {
                imagePreivew3.Source = bmpImage;
                i++;
            }

            else
            {
                imagePreivew.Source = bmpImage;
                i = 1;
            }

        }

        SpeechRecognizer recognizer;


    }
}
