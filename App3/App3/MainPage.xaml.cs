using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static uint HResultPrivacyStatementDeclined = 0x80045509;

        /// <summary>
        /// the HResult 0x8004503a typically represents the case where a recognizer for a particular language cannot
        /// be found. This may occur if the language is installed, but the speech pack for that language is not.
        /// See Settings -> Time & Language -> Region & Language -> *Language* -> Options -> Speech Language Options.
        /// </summary>
        private static uint HResultRecognizerNotFound = 0x8004503a;

        private SpeechRecognizer speechRecognizer;
      //  private CoreDispatcher dispatcher;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        private IAsyncOperation<SpeechRecognitionResult> recognitionOperation;

        private SpeechSynthesizer synthesizer;
       // private ResourceContext speechContext;
        //private ResourceMap speechResourceMap;
        public MainPage()
        {
            this.InitializeComponent();
           synthesizer = new SpeechSynthesizer();


            

            //speechContext = ResourceContext.GetForCurrentView();
            //speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            //speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");



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
            
            //this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            //this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

            //this.recognizer.UIOptions.AudiblePrompt = "Say whatever you like, I'm listening";
            //this.recognizer.UIOptions.ExampleText = "The quick brown fox jumps over the lazy dog";
            //this.recognizer.UIOptions.ShowConfirmation = true;
            //this.recognizer.UIOptions.IsReadBackEnabled = true;
            //this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

            //var result = await this.recognizer.RecognizeWithUIAsync();

        }

        private async Task InitializeRecognizer(Language recognizerLanguage)
        {
            if (speechRecognizer != null)
            {
                // cleanup prior to re-initializing this scenario.
                //speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;

                //this.speechRecognizer.Dispose();
                //this.speechRecognizer = null;
            }
            try
            {
                // Create an instance of SpeechRecognizer.
                speechRecognizer = new SpeechRecognizer(recognizerLanguage);

                // Provide feedback to the user about the state of the recognizer.
        

                // Add a list constraint to the recognizer.
                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>()
                        {
                        speechResourceMap.GetValue("ListGrammarGoHome", speechContext).ValueAsString
                        }, "Home"));
                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>()
                        {
                        speechResourceMap.GetValue("ListGrammarGoToContosoStudio", speechContext).ValueAsString
                        }, "GoToContosoStudio"));
                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>()
                        {
                        speechResourceMap.GetValue("ListGrammarShowMessage", speechContext).ValueAsString,
                        speechResourceMap.GetValue("ListGrammarOpenMessage", speechContext).ValueAsString
                        }, "Message"));
                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>()
                        {
                        speechResourceMap.GetValue("ListGrammarSendEmail", speechContext).ValueAsString,
                        speechResourceMap.GetValue("ListGrammarCreateEmail", speechContext).ValueAsString
                        }, "Email"));
                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>()
                        {
                        speechResourceMap.GetValue("ListGrammarCallNitaFarley", speechContext).ValueAsString,
                        speechResourceMap.GetValue("ListGrammarCallNita", speechContext).ValueAsString
                        }, "CallNita"));
                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>()
                        {
                        speechResourceMap.GetValue("ListGrammarCallWayneSigmon", speechContext).ValueAsString,
                        speechResourceMap.GetValue("ListGrammarCallWayne", speechContext).ValueAsString
                        }, "CallWayne"));

                // RecognizeWithUIAsync allows developers to customize the prompts.
                string uiOptionsText = string.Format("Try saying '{0}', '{1}' or '{2}'",
                    speechResourceMap.GetValue("ListGrammarGoHome", speechContext).ValueAsString,
                    speechResourceMap.GetValue("ListGrammarGoToContosoStudio", speechContext).ValueAsString,
                    speechResourceMap.GetValue("ListGrammarShowMessage", speechContext).ValueAsString);
                speechRecognizer.UIOptions.ExampleText = uiOptionsText;
                //helpTextBlock.Text = string.Format("{0}\n{1}",
                //    speechResourceMap.GetValue("ListGrammarHelpText", speechContext).ValueAsString,
                //    uiOptionsText);

                // Compile the constraint.
                SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

                // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
                if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                {
                    // Disable the recognition buttons.
                    //btnRecognizeWithUI.IsEnabled = false;
                    //btnRecognizeWithoutUI.IsEnabled = false;

                    // Let the user know that the grammar didn't compile properly.
                    txtResults.Visibility = Visibility.Visible;
                    txtResults.Text = "Unable to compile grammar.";
                }
                else
                {
                    //btnRecognizeWithUI.IsEnabled = true;
                    //btnRecognizeWithoutUI.IsEnabled = true;

                    txtResults.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == HResultRecognizerNotFound)
                {
                    //btnRecognizeWithUI.IsEnabled = false;
                    //btnRecognizeWithoutUI.IsEnabled = false;

                    txtResults.Visibility = Visibility.Visible;
                    txtResults.Text = "Speech Language pack for selected language not installed.";
                }
                else
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                    await messageDialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Handle SpeechRecognizer state changed events by updating a UI component.
        /// </summary>
        /// <param name="sender">Speech recognizer that generated this status event</param>
        /// <param name="args">The recognizer's status</param>
        



        async void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            await recognize();

        }

        async Task recognize() {

            //if (this.recognizer == null)
            //{
            //    this.recognizer = new SpeechRecognizer();
            //}            


            //await this.recognizer.CompileConstraintsAsync();
            //var result = await this.recognizer.RecognizeAsync();

            //if (result.Text == "next")
            //{
            //    this.Frame.Navigate(typeof(BlankPage1), null);
            //}

            //else
            //{
            //    await play("i dont understand");
            //}

            try
            {
                //if (this.recognizer == null)
                //{
                //    this.recognizer = new SpeechRecognizer();
                //}
                Language speechLanguage = SpeechRecognizer.SystemSpeechLanguage;
                string langTag = speechLanguage.LanguageTag;
                speechContext = ResourceContext.GetForCurrentView();
                speechContext.Languages = new string[] { langTag };

                speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationSpeechResources");

                await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);


                recognitionOperation = speechRecognizer.RecognizeWithUIAsync();

                var speechRecognitionResult = await recognitionOperation;


                //await this.recognizer.CompileConstraintsAsync();
                //var speechRecognitionResult = await this.recognizer.RecognizeWithUIAsync();

                //recognitionOperation = speechRecognizer.RecognizeWithUIAsync();

                //SpeechRecognitionResult speechRecognitionResult = await recognitionOperation;
                // If successful, display the recognition result.
                txtResults.Text = string.Format("Heard: '{0}'", speechRecognitionResult.Text);
            }
            //    if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
            //    {
            //        string tag = "unknown";
            //        if (speechRecognitionResult.Constraint != null)
            //        {
            //            // Only attempt to retreive the tag if we didn't hit the garbage rule.
            //            tag = speechRecognitionResult.Constraint.Tag;
            //        }

            //        //heardYouSayTextBlock.Visibility = resultTextBlock.Visibility = Visibility.Visible;
            //        txtResults.Text = string.Format("Heard: '{0}', (Tag: '{1}', Confidence: {2})", speechRecognitionResult.Text, tag, speechRecognitionResult.Confidence.ToString());
            //    }
            //    else
            //    {
            //        txtResults.Visibility = Visibility.Visible;
            //        txtResults.Text = string.Format("Speech Recognition Failed, Status: {0}", speechRecognitionResult.Status.ToString());
            //    }
            //}
            //catch (TaskCanceledException exception)
            //{
            //    // TaskCanceledException will be thrown if you exit the scenario while the recognizer is actively
            //    // processing speech. Since this happens here when we navigate out of the scenario, don't try to 
            //    // show a message dialog for this exception.
            //    System.Diagnostics.Debug.WriteLine("TaskCanceledException caught while recognition in progress (can be ignored):");
            //    System.Diagnostics.Debug.WriteLine(exception.ToString());
            //}
            catch (Exception exception)
            {
                // Handle the speech privacy policy error.
                if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                {
                    txtResults.Visibility = Visibility.Visible;
                    txtResults.Text = "The privacy statement was declined.";
                }
                else
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
                    await messageDialog.ShowAsync();
                }
            }

        }

        async Task play(string welcome)
        {
            SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(welcome);
            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
        }


        //async void button_Click(object sender, RoutedEventArgs e)
        //{

        //    this.recognizer = new SpeechRecognizer();
        //    await this.recognizer.CompileConstraintsAsync();

        //    this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
        //    this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

        //    this.recognizer.UIOptions.AudiblePrompt = "Say whatever you like, I'm listening";
        //    this.recognizer.UIOptions.ExampleText = "The quick brown fox jumps over the lazy dog";
        //    this.recognizer.UIOptions.ShowConfirmation = true;
        //    this.recognizer.UIOptions.IsReadBackEnabled = true;
        //    this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

        //    var result = await this.recognizer.RecognizeWithUIAsync();

        //    if (result.Text == "next")
        //    {
        //        this.Frame.Navigate(typeof(BlankPage1), null);
        //    }


        //if (result != null)
        //{
        //    StringBuilder builder = new StringBuilder();

        //    builder.AppendLine(
        //      $"I have {result.Confidence} confidence that you said [{result.Text}] " +
        //      $"and it took {result.PhraseDuration.TotalSeconds} seconds to say it " +
        //      $"starting at {result.PhraseStartTime:g}");

        //    var alternates = result.GetAlternates(10);

        //    builder.AppendLine(
        //      $"There were {alternates?.Count} alternates - listed below (if any)");

        //    if (alternates != null)
        //    {
        //        foreach (var alternate in alternates)
        //        {
        //            builder.AppendLine(
        //              $"Alternate {alternate.Confidence} confident you said [{alternate.Text}]");
        //        }
        //    }
        //    this.txtResults.Text = builder.ToString();
        //}
        // }


        //SpeechRecognizer recognizer;
        //SpeechRecognizer recognizer;
    }
}
