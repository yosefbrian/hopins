using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hopins
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class addProductName : Page
    {
        private CoreDispatcher dispatcher;
        private SpeechRecognizer speechRecognizer;
        private static uint HResultRecognizerNotFound = 0x8004503a;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private bool isListening;
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        public addProductName()
        {
            this.InitializeComponent();
            isListening = false;
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await disableSpeech();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
            if (permissionGained)
            {
               // btnContinuousRecognize.IsEnabled = true;
                speechContext = ResourceContext.GetForCurrentView();
                speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");
                play(question.Text);
            }
            else
            {
                this.resultTextBlock.Visibility = Visibility.Visible;
                this.resultTextBlock.Text = "Permission to access capture resources was not given by the user, reset the application setting in Settings->Privacy->Microphone.";
                btnContinuousRecognize.IsEnabled = false;

            }

            var parameters = (addProduct.imagePath)e.Parameter;
            textBox1.Text = parameters.path;


        }


        private void next_Click(object sender, RoutedEventArgs e)
        {
            var yeah = new data();
            yeah.name = textBox.Text;
            yeah.path = textBox1.Text;
            this.Frame.Navigate(typeof(addProductCategory), yeah);
        }

        async void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
            ContinuousRecognize_Click(this, new RoutedEventArgs());
        }

        private async void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
            });
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

                if (textBox.IsEnabled == true)
                {
                        string productName = args.Result.Text;
                        textBox.Text = args.Result.Text;

                        if (args.Result.Text != null)
                        {
                            await disableSpeech();
                            textBox.IsEnabled = false;
                            string help = string.Format("The name of your product is {0} , if the name is correct, say next, if you want to repeat, say undo", productName);
                            play(help);
                        }
                    }

                    else if (textBox.IsEnabled == false)
                    {
                    if (args.Result.Text == "undo")
                    {
                        await disableSpeech();
                        textBox.IsEnabled = true;
                        play("what is the name of your product?");
                    }

                    else if (args.Result.Text == "next")
                    {
                        next_Click(this, new RoutedEventArgs());
                    }

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


        private async Task disableSpeech()
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
                this.speechRecognizer = new SpeechRecognizer(recognize);

                if (textBox.IsEnabled == true)
                {
                    var grammar = new[] { "Brown Wallet", "Elephant Puzzle" };
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
                else if (textBox.IsEnabled == false)
                {
                   
                        var grammar = new[] { "next", "undo", "home", "help" };
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
                       // await speechRecognizer.RecognizeAsync();
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

        public class data
        {
            public string path { get; set; }
            public string name { get; set; }
        }



    }
}
