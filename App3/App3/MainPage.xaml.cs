using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
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
        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;
        public MainPage()
        {
            this.InitializeComponent();
            synthesizer = new SpeechSynthesizer();

            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");

          

        }

        private async void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            try
            {
                string welcome = "welcome to the hopins store";
                SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(welcome);
                media.AutoPlay = true;
                media.SetSource(synthesisStream, synthesisStream.ContentType);
                media.Play();
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


            this.recognizer = new SpeechRecognizer();
            await this.recognizer.CompileConstraintsAsync();

            //this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            //this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

            //this.recognizer.UIOptions.AudiblePrompt = "Say whatever you like, I'm listening";
            //this.recognizer.UIOptions.ExampleText = "The quick brown fox jumps over the lazy dog";
            //this.recognizer.UIOptions.ShowConfirmation = true;
            //this.recognizer.UIOptions.IsReadBackEnabled = true;
            //this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

            //var result = await this.recognizer.RecognizeWithUIAsync();

            var result = await this.recognizer.RecognizeAsync();      

            if (result.Text == "next")
            {
                this.Frame.Navigate(typeof(BlankPage1), null);
            }

            else
            {
                txtResults.Text = result.Text;
            }


        }





        async void button_Click(object sender, RoutedEventArgs e)
        {

            this.recognizer = new SpeechRecognizer();
            await this.recognizer.CompileConstraintsAsync();

            this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

            this.recognizer.UIOptions.AudiblePrompt = "Say whatever you like, I'm listening";
            this.recognizer.UIOptions.ExampleText = "The quick brown fox jumps over the lazy dog";
            this.recognizer.UIOptions.ShowConfirmation = true;
            this.recognizer.UIOptions.IsReadBackEnabled = true;
            this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

            var result = await this.recognizer.RecognizeWithUIAsync();

            if (result.Text == "next")
            {
                this.Frame.Navigate(typeof(BlankPage1), null);
            }


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
        }

        async void yeah()
        {
            

        }

        SpeechRecognizer recognizer;
    }
}
