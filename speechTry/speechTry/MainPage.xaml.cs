using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using System.Threading.Tasks;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace speechTry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private SpeechRecognizer _speechRecognizer;

        

        private async Task<SpeechRecognitionResult> SpeechRecognizeAsync()
        {
            if (_speechRecognizer == null)
            {
                // Create an instance of SpeechRecognizer.
                _speechRecognizer = new SpeechRecognizer();

                var songs = new[] { "order", "product", "manage", "capture", "home" };

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
            return await _speechRecognizer.RecognizeWithUIAsync();
        }

        private async void Speech_Click(object sender, RoutedEventArgs e)
        {
            // Get the recognition result.
            var speechRecognitionResult = await SpeechRecognizeAsync();

            // Pops up a dialog to show the result.

            if (speechRecognitionResult.Text == "My product")
            {
                this.Frame.Navigate(typeof(BlankPage1), null);
            }
            //var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");
            //await messageDialog.ShowAsync();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage1), null);
        }
    }
}
