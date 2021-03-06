﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
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
using static hopins.addProductMaterial;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hopins
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class addProductSummary : Page
    {
        private CoreDispatcher dispatcher;
        private SpeechRecognizer speechRecognizer;
        private static uint HResultRecognizerNotFound = 0x8004503a;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private bool isListening;
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        public addProductSummary()
        {
            this.InitializeComponent();
            isListening = false;
        }

       

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
                btnContinuousRecognize.IsEnabled = true;
                speechContext = ResourceContext.GetForCurrentView();
                speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");
               
            }
            else
            {
                this.resultTextBlock.Visibility = Visibility.Visible;
                this.resultTextBlock.Text = "Permission to access capture resources was not given by the user, reset the application setting in Settings->Privacy->Microphone.";
                btnContinuousRecognize.IsEnabled = false;

            }

            base.OnNavigatedTo(e);

            var parameters = (addProductPrice.ProductParams)e.Parameter;
            productName.Text = parameters.Name;
            productCategory.Text = parameters.Category;
            productMaterial.Text = parameters.Material;
            productPrice.Text = parameters.Price;
            textBlock.Text = parameters.Path;
            // image.Source = new BitmapImage(new Uri(parameters.Path));
            string fileName = textBlock.Text;
            StorageFolder myfolder = ApplicationData.Current.LocalFolder;
            BitmapImage bitmapImage = new BitmapImage();
            StorageFile file = await myfolder.GetFileAsync(fileName);
            image.Source = new BitmapImage(new Uri(file.Path));

            string summary = string.Format("Summary : the name of this product is {0}, the type of this product is {1}, the material of this product is {2}, the price of this product is {3} dollar, if you want to publish, say publish", productName.Text, productCategory.Text, productMaterial.Text, productPrice.Text);
            play(summary);
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

        private async void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            try {

                yeah.IsActive = true;

            string fileName = textBlock.Text;
            StorageFolder myfolder = ApplicationData.Current.LocalFolder;
            BitmapImage bitmapImage = new BitmapImage();
            StorageFile file = await myfolder.GetFileAsync(fileName);


            using (var client1 = new System.Net.Http.HttpClient())
            {
                string category = productCategory.Text;
                string Material = productMaterial.Text;
                string deskripsi = string.Format("The type of this product is {0} and the material of this product is {1} dollar", category, Material);
                var values = new Dictionary<string, string>
                {
                    { "nama", productName.Text },
                    { "deskripsi", deskripsi},
                    { "harga", productPrice.Text},
                    { "gambar", file.Name }
                };

                var content1 = new FormUrlEncodedContent(values);

                var response1 = await client1.PostAsync("http://hopins16.azurewebsites.net/hopins/public/create", content1);

                var responseString1 = await response1.Content.ReadAsStringAsync();
            }

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new Uri("http://hopins16.azurewebsites.net/hopins/public");
            MultipartFormDataContent form = new MultipartFormDataContent();
            HttpContent content = new StringContent("fileToUpload");
            form.Add(content, "fileToUpload");
            var stream = await file.OpenStreamForReadAsync();
            content = new StreamContent(stream);

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "fileToUpload",
                FileName = file.Name
            };
            form.Add(content);
            var response = await client.PostAsync("http://hopins16.azurewebsites.net/hopins/public/upload", form);
            var responseString = response.Content.ReadAsStringAsync().Result;

                this.Frame.Navigate(typeof(addProductSuccess), null);
            }

            catch(Exception ex)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                await messageDialog.ShowAsync();
            }


        }

        private void productMaterial_Copy_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }


        private void backButton()
        {
            var currentView = SystemNavigationManager.GetForCurrentView();

            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += backButton_Tapped;
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {

            if (Frame.CanGoBack) Frame.GoBack();

        }

        async void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            ContinuousRecognize_Click(this, new RoutedEventArgs());
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
                var grammar = new[] { "order", "product", "manage", "capture", "home", "exit", "help", "publish" };
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
                        this.Frame.Navigate(typeof(myProduct), null);
                        backButton();
                    }

                    else if (args.Result.Text == "order")
                    {
                        this.Frame.Navigate(typeof(myProduct), null);
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

                    else if (args.Result.Text == "publish")
                    {
                        btnPublish_Click(this, new RoutedEventArgs());
                    }


                    else if (args.Result.Text == "exit")
                    {
                        Application.Current.Exit();
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








    }
}
