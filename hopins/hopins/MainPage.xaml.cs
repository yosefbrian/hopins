using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace hopins
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private SpeechRecognizer _speechRecognizer;

        public MainPage()
        {
            this.InitializeComponent();
            synthesizer = new SpeechSynthesizer();

            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this._speechRecognizer.Dispose();
            this._speechRecognizer = null;

        }


        private async void Page_Loaded(Object sender, RoutedEventArgs e)
        {
          

            try
            {
                await play("welcome to the hopins store");
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
            try {
                var speechRecognitionResult = await recognize();

                if (speechRecognitionResult.Text == "product")
                {
                    this.Frame.Navigate(typeof(addProduct), null);
                    Test.Text = speechRecognitionResult.Text;
                    backButton();
                }

                else if (speechRecognitionResult.Text == "manage")
                {
                    this.Frame.Navigate(typeof(myProduct), null);
                    backButton();
                    Test.Text = speechRecognitionResult.Text;
                }
                else if (speechRecognitionResult.Text == "order")
                {
                    this.Frame.Navigate(typeof(orderManagement), null);
                    backButton();
                    Test.Text = speechRecognitionResult.Text;
                }
                else if (speechRecognitionResult.Text == "exit")
                {
                    Test.Text = speechRecognitionResult.Text;
                    Application.Current.Exit();

                }
                else if (speechRecognitionResult.Text == null)
                {
                    Test.Text = speechRecognitionResult.Text;
                    await play("Iam waiting for your response");
                }
                else
                {
                    Test.Text = speechRecognitionResult.Text;
                    await play("i dont understand, Iam waiting for your response");
                }
            }
            catch (Exception)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Unable to synthesize text");
                await messageDialog.ShowAsync();
            }

        }

        private async Task<SpeechRecognitionResult> recognize()
        {

            // if (this.recognizer == null)
            // {
            //     this.recognizer = new SpeechRecognizer();
            // }


            // await this.recognizer.CompileConstraintsAsync();
            // var result = await this.recognizer.RecognizeWithUIAsync();

            // if (result.Text == "show" || result.Text == "saw" || result.Text == "sow")
            // {
            //     Test.Text = result.Text;
            //     this.Frame.Navigate(typeof(myProduct), null);
            // }

            // else if (result.Text == "add" || result.Text == "at" || result.Text == "app"|| result.Text == "product")
            // {
            //     Test.Text = result.Text;
            //     this.Frame.Navigate(typeof(addProduct), null);
            // }
            //else if (result.Text == "manage" || result.Text == "order" || result.Text == "management")
            // {
            //     Test.Text = result.Text;
            //     this.Frame.Navigate(typeof(orderManagement), null);
            // }
            // else
            // {
            //     Test.Text = result.Text;
            //     await play("i dont understand");

            // }
            //this._speechRecognizer.Dispose();
            //this._speechRecognizer = null;

            if (_speechRecognizer == null)
            {
                // Create an instance of SpeechRecognizer.
                _speechRecognizer = new SpeechRecognizer();
          

                var songs = new[] { "order", "product", "manage", "capture", "home", "exit" };

                // Generates the collection which we expect user will say one of.

                // Create an instance of the constraint.
                // Pass the collection and an optional tag to identify.
                var playConstraint = new SpeechRecognitionListConstraint(songs);

                // Add it into teh recognizer
                _speechRecognizer.Constraints.Add(playConstraint);

                // Then add the constraint for pausing and resuming.

                //var pauseConstraint = new SpeechRecognitionListConstraint(new[] { "Pause", "Resume" }, "pauseAndResume");
                //_speechRecognizer.Constraints.Add(pauseConstraint);

                // Compile the dictation grammar by default.
                await _speechRecognizer.CompileConstraintsAsync();
            }

            // Start recognition and return the result.
            return await _speechRecognizer.RecognizeAsync();

        }


        async Task play(string welcome)
        {
            SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(welcome);
            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
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
            this.Frame.Navigate(typeof(myProduct), null);
            backButton();
        }

        private void btOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(orderManagement), null);
            backButton();
            
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {

            if (Frame.CanGoBack) Frame.GoBack();

        }

        //SpeechRecognizer recognizer;
    }
}
