using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace speechTry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
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

                // Get a file.
                var grammarFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Light.grxml"));

                // Create the constraint from the file.
                var srgsConstraint = new SpeechRecognitionGrammarFileConstraint(grammarFile, "light");

                // Add the constraint.
                _speechRecognizer.Constraints.Add(srgsConstraint);

                // Compile the dictation grammar by default.
                await _speechRecognizer.CompileConstraintsAsync();
            }

            // Start recognition and return the result.
            return await _speechRecognizer.RecognizeWithUIAsync();
        }

        private async void Speech_Click(object sender, RoutedEventArgs e)
        {
            SpeechButton.IsEnabled = false;
            // Get the recognition result.
            try { 

            var speechRecognitionResult = await SpeechRecognizeAsync();

            if (speechRecognitionResult.Constraint == null) return;
            switch (speechRecognitionResult.Constraint.Tag)
            {
                case "light":
                    Control(speechRecognitionResult.RulePath.ToList());
                    break;
                //case "play":
                //    // Play something.
                //    var songName = speechRecognitionResult.Text.Substring("Play ".Length).Trim();
                //    break;
                //case "pauseAndResume":
                //    // Pause something.
                //    break;
            }

            // Pops up a dialog to show the result.
            var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");
            await messageDialog.ShowAsync();
            }

            catch (Exception exception)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
                await messageDialog.ShowAsync();
            }
        }

        private void Control(IList<string> path)
        {
            if (path.Count < 2) { return; }

            switch (path[1])
            {
                case "TurnOn":
                    // Turn on.
                    break;
                case "TurnOff":
                    // Turn off.
                    break;
            }
        }



    }
}
