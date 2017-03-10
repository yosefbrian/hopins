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
            var result = await this.recognizer.RecognizeWithUIAsync();

            if (result.Text == "show" || result.Text == "saw" || result.Text == "sow")
            {
                Test.Text = result.Text;
                this.Frame.Navigate(typeof(myProduct), null);
            }

            else if (result.Text == "add" || result.Text == "at" || result.Text == "app"|| result.Text == "product")
            {
                Test.Text = result.Text;
                this.Frame.Navigate(typeof(addProduct), null);
            }
           else if (result.Text == "manage" || result.Text == "order" || result.Text == "management")
            {
                Test.Text = result.Text;
                this.Frame.Navigate(typeof(orderManagement), null);
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

        private void textBlock1_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        SpeechRecognizer recognizer;
    }
}
