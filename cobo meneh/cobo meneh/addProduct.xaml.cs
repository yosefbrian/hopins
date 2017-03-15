using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace cobo_meneh
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class addProduct : Page
    {
        public addProduct()
        {
            this.InitializeComponent();

            this.InitializeComponent();
            synthesizer = new SpeechSynthesizer();
            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };
            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");
        }

        private CoreDispatcher dispatcher;
        private SpeechRecognizer speechRecognizer;
        private static uint HResultRecognizerNotFound = 0x8004503a;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private bool isListening;
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        Windows.Media.Capture.MediaCapture captureManager;


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            

            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            try { 
                await intializeCamera();
            }
            catch(Exception ex)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                await messageDialog.ShowAsync();
            }
                string welcome = "Welcome to the add product page. To capture a photo of your product, say capture";
            

            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
                btnContinuousRecognize.IsEnabled = true;
                speechContext = ResourceContext.GetForCurrentView();
                speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");
                play(welcome);
            }
            else
            {
                this.resultTextBlock.Visibility = Visibility.Visible;
                this.resultTextBlock.Text = "Permission to access capture resources was not given by the user, reset the application setting in Settings->Privacy->Microphone.";
                btnContinuousRecognize.IsEnabled = false;

            }
        }


        private async Task intializeCamera()
        {
            if (captureManager == null)
            {
                captureManager = new MediaCapture();
                await captureManager.InitializeAsync();
            }

            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();

            try
            {
                play("This is add product page");
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
            await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            ContinuousRecognize_Click(this, new RoutedEventArgs());
        }



        private async void play(string welcome)
        {
            try
            {
                SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(welcome);
                media.AutoPlay = true;
                media.SetSource(synthesisStream, synthesisStream.ContentType);
                media.Play();
            }

            catch (Exception ex)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                await messageDialog.ShowAsync();
            }
        }


        private async Task InitializeRecognizer(Language recognize)
        {


            if (speechRecognizer != null)
            {
                speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }
            try
            {
                this.speechRecognizer = new SpeechRecognizer();
                var grammar = new[] { "order", "product", "manage", "capture", "home", "exit", "help", "back" };
                var playConstraint = new SpeechRecognitionListConstraint(grammar);
                speechRecognizer.Constraints.Add(playConstraint);
                speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
                SpeechRecognitionCompilationResult result = await speechRecognizer.CompileConstraintsAsync();

                if (result.Status != SpeechRecognitionResultStatus.Success)
                {
                    btnContinuousRecognize.IsEnabled = false;


                    resultTextBlock.Visibility = Visibility.Visible;
                    resultTextBlock.Text = "Unable to compile grammar.";
                }
                else
                {
                    btnContinuousRecognize.IsEnabled = true;
                    resultTextBlock.Visibility = Visibility.Collapsed;
                    speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
                    speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == HResultRecognizerNotFound)
                {
                    btnContinuousRecognize.IsEnabled = false;
                    resultTextBlock.Visibility = Visibility.Visible;
                    resultTextBlock.Text = "Speech Language pack for selected language not installed.";
                }
                else
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                    await messageDialog.ShowAsync();
                }
            }

        }


        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this.speechRecognizer != null)
            {
                if (isListening)
                {
                    await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                    isListening = false;
                }
                speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }
        }


        private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            if (args.Status != SpeechRecognitionResultStatus.Success)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ContinuousRecoButtonText.Text = " Continuous Recognition";
                    isListening = false;
                });
            }
        }


        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
                args.Result.Confidence == SpeechRecognitionConfidence.High || args.Result.Confidence == SpeechRecognitionConfidence.Low)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    resultTextBlock.Visibility = Visibility.Visible;
                    resultTextBlock.Text = args.Result.Text;
                    if (args.Result.Text == "product")
                    {
                        this.Frame.Navigate(typeof(addProduct), null);
                        backButton();
                    }

                    else if (args.Result.Text == "manage")
                    {
                       // this.Frame.Navigate(typeof(myProduct), null);
                        backButton();
                    }

                    else if (args.Result.Text == "order")
                    {
                        //this.Frame.Navigate(typeof(myProduct), null);
                        backButton();
                    }

                    else if (args.Result.Text == "back" || args.Result.Text == "home")
                    {
                        this.Frame.Navigate(typeof(MainPage), null);
                        backButton();
                    }
                 

                    else if (args.Result.Text == "help")
                    {

                        if (this.speechRecognizer != null)
                        {
                            if (isListening)
                            {
                                await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                                isListening = false;
                            }
                            speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                            this.speechRecognizer.Dispose();
                            this.speechRecognizer = null;
                        }
                        string help = "welcome to the hopins store. To add new product say product, to manage your product say manage, to manage your order say order, to repeat the instruction, say help.";
                        play(help);
                    }

                    else if (args.Result.Text == "exit")
                    {
                        CoreApplication.Exit();
                    }
                    else if (args.Result.Text == "capture")
                    {

                        if (this.speechRecognizer != null)
                        {
                            if (isListening)
                            {
                                await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                                isListening = false;
                            }
                            speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
                            this.speechRecognizer.Dispose();
                            this.speechRecognizer = null;
                        }
                        string help = "image captured, if you want to capture another photo, say capture. If you are finish, say next";
                        play(help);
                        CapturePhoto_Click(this, new RoutedEventArgs());
                        
                    }
                    else
                    {
                        resultTextBlock.Text = "I dont understand what you said";
                    }
                });
            }

            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    resultTextBlock.Visibility = Visibility.Visible;
                    resultTextBlock.Text = "sorry, I didnt catch that";
                });
            }

        }

        private async void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
            });
        }


        public async void ContinuousRecognize_Click(object sender, RoutedEventArgs e)
        {
            btnContinuousRecognize.IsEnabled = false;
            if (isListening == false)
            {
                if (speechRecognizer.State == SpeechRecognizerState.Idle)
                {
                    try
                    {
                        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
                        ContinuousRecoButtonText.Text = " Stop Listening";
                        isListening = true;
                        media.Stop();
                    }
                    catch (Exception ex)
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                        await messageDialog.ShowAsync();
                    }
                }
            }
            else
            {
                isListening = false;
                ContinuousRecoButtonText.Text = " Continuous Recognition";
                resultTextBlock.Visibility = Visibility.Collapsed;
                if (speechRecognizer.State != SpeechRecognizerState.Idle)
                {
                    try
                    {
                        await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                    }
                    catch (Exception ex)
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                        await messageDialog.ShowAsync();
                    }
                }
            }
            btnContinuousRecognize.IsEnabled = true;
        }

        private void backButton()
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += backButton_Tapped;
        }


        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
           this.Frame.Navigate(typeof(addProduct), null);
            backButton();
        }

        private void btProduct_Click(object sender, RoutedEventArgs e)
        {
           //this.Frame.Navigate(typeof(myProduct), null);
            backButton();
        }

        private void btOrder_Click(object sender, RoutedEventArgs e)
        {
          //  this.Frame.Navigate(typeof(orderManagement), null);
            backButton();

        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {

            if (Frame.CanGoBack) Frame.GoBack();

        }

        int i = 0;
        async private void CapturePhoto_Click(object sender, RoutedEventArgs e)
        {
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.GenerateUniqueName);
            await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(addProductName), null);
        }
    }
}
