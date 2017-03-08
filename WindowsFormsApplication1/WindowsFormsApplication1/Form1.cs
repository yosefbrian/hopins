using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer ss = new SpeechSynthesizer();
        PromptBuilder pb = new PromptBuilder();
        SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        Choices clist = new Choices();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            clist.Add(new string[] { "Hello", "how are you?", "what is the current time", "open chrome", "thank you", "close" });
            Grammar gr = new Grammar(new GrammarBuilder(clist));

            try{
                sre.RequestRecognizerUpdate();
                sre.LoadGrammar(gr);
                sre.SpeechRecognized += sre_SpeechRecognized;
                sre.SetInputToDefaultAudioDevice();
                sre.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
           switch(e.Result.Text.ToString())
            {
                case "hello":
                    ss.SpeakAsync("hello yudha");
                    break;
                case "how are you":
                    ss.SpeakAsync("i am doing great, how about you");
                    break;
                case "what is the current time":
                    ss.SpeakAsync("current time is" + DateTime.Now.ToLongTimeString());
                    break;
                case "thank you":
                    ss.SpeakAsync("pleasure is mine yudha");
                    break;
                case "open chrome":
                    Process.Start("chrome", "https://ybrian.net");
                    break;
                case "close":
                    Application.Exit();
                    break;

            }

            txtContents.Text += e.Result.Text.ToString() + Environment.NewLine;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            sre.RecognizeAsyncStop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
}
