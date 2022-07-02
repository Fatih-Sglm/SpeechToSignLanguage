using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Grpc.Auth;
using NAudio.Wave;
using SpeechToSignLanguage.Client.Functions;
using SpeechToSignLanguage.Client.Functions.Video;
using SpeechToSignLanguage.Client.Functions.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeechToSignLanguage.Client.Forms
{
    public partial class Main : Form
    {
        private readonly BufferedWaveProvider bwp;
        WaveIn waveIn;
        readonly WaveOut waveOut;
        WaveFileWriter writer;
        WaveFileReader reader;
        public string UserName { get; set; }
        Guid value = Guid.NewGuid();
        Stopwatch watch = new System.Diagnostics.Stopwatch();
        readonly WordFunction wordFunction = new WordFunction();
        readonly VideoFunciton vf = new VideoFunciton();
        readonly GeneralFunction gf = new GeneralFunction();
        readonly NumberFunction nf = new NumberFunction();


        public Main(string txt)
        {
            InitializeComponent();
            waveOut = new WaveOut();
            waveIn = new WaveIn();
            UserName = txt;
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(WaveIn_DataAvailable);
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
            bwp = new BufferedWaveProvider(waveIn.WaveFormat)
            {
                DiscardOnBufferOverflow = true
            };
            UserNameLabel.Text += UserName;
        }
        private void BtnRecord_ClickAsync(object sender, EventArgs e)
        {

            if (NAudio.Wave.WaveIn.DeviceCount < 1)
            {
                MessageBox.Show("An active microphone was not found..Microphone Not Connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            txtSentence.Text = "We are listening to you";
            btnRecord.Enabled = false;
            try
            {
                if (gf.CheckForInternetConnection())
                {


                    waveIn.StartRecording();
                    if (CustomMessageBox.Show("Your Voice Is Now Recording. Press the Ok \n key to translate, press the Cancel key \n to cancel", "Stop Recording", "Cancel") == DialogResult.Yes)
                    {
                        watch.Start();
                        AfterRecording();
                    }
                    else
                    {
                        SpeechToTextAfter();
                    }
                }
                else
                {
                    MessageBox.Show("Please Check Your Internet Connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SpeechToTextAfter();
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Please Check Your Microphone Permission", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SpeechToTextAfter();
            }

        }
        private async void BtnConvert_ClickAsync(object sender, EventArgs e)
        {

            var sentence = txtSentence.Text;
            sentence = sentence.Replace("\n", " ").Replace("\r", " ");
            await PlayButtonAsync(sentence);

        }
        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            try
            {
                waveOut.Stop();
                reader.Close();
                reader = null;
            }
            catch
            {

            }
        }
        void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);

        }
        private async void PlayVideonScreen(List<string> wordsList)
        {

            if (wordsList.Count > 0)
            {

                for (int i = 0; i < wordsList.Count; i++)
                {
                    axWindowsMediaPlayer1.URL = wordsList[i];
                    if (vf.CheckVideoInclude(wordsList[i]))
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                        axWindowsMediaPlayer1.settings.volume = 0;
                        int duration = vf.Duration(wordsList[i]);
                        //await ShowCurrentWord(duration, i);
                        await Task.Delay(duration);
                    }
                    else
                    {
                        MessageBox.Show("This word is not in our Dictionary.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                btnConvert.Enabled = true;
                txtSentence.Select(0, txtSentence.Text.Length);
                txtSentence.SelectionColor = Color.Black;
                txtSentence.Select(0, 0);
            }

            wordsList.RemoveAll(wordsList.Contains);
        }
        //public string bla(string s)
        //{
        //    s.Replace("C:\\Users\\fatih\\source\\repos\\SpeechToSignLanguage\\presentation\\SpeechToSignLanguage.Client\\Videos\\", "");
        //    return s;
        //}
        public async Task ShowCurrentWord(int duration, int i)
        {
            string[] words = txtSentence.Text.Replace("\n", "").Split(' ');

            var s = txtSentence.Find(words[i]);
            txtSentence.Select(s, words[i].Length);
            txtSentence.SelectionColor = Color.Green;
            txtSentence.SelectionFont = new Font("Unispace", 18);
            await Task.Delay(duration);
            txtSentence.SelectionColor = Color.Black;
            txtSentence.SelectionFont = new Font("Times New Roman", 14);
        }


        #region ButtonHover


        private void BtnRecord_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Record the Voice", btnRecord);

        }
        private void BtnConvert_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Convert the text", btnConvert);
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }
        private void BtnStop_Click(object sender, EventArgs e)
        {
            var prc = Process.GetProcessesByName("axWindowsMediaPlayer1");
            if (prc.Length > 0) prc[prc.Length - 1].Kill();

        }
        #endregion
        #region RecordVoice

        private async void AfterRecording()
        {
            gf.CreateDirectory(UserName);
            string output = "C:\\record\\" + UserName + "\\" + value.ToString() + ".flac";
            this.Cursor = Cursors.WaitCursor;
            await SpeechToTextAsync(output);
        }
        #endregion
        #region SpeechToText


        private async Task SpeechToTextAsync(string output)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            waveIn.StopRecording();
            writer = new WaveFileWriter(output, waveIn.WaveFormat);
            byte[] buffer = new byte[bwp.BufferLength];
            int offset = 0;
            int count = bwp.BufferLength;
            var read = bwp.Read(buffer, offset, count);
            if (count > 0)
            {
                writer.Write(buffer, offset, read);
            }
            waveIn.Dispose();
            waveIn = null;
            writer.Close();
            writer = null;
            reader = new WaveFileReader(output);
            waveOut.Init(reader);
            waveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(WaveOut_PlaybackStopped);
            reader.Close();
            if (File.Exists(output))
            {
                if (gf.CheckForInternetConnection())
                {
                    GoogleCredential credential;
                    using (var stream = new FileStream("../../key.json", FileMode.Open, FileAccess.Read))
                    {
                        credential = GoogleCredential.FromStream(stream);
                    }
                    var speech = new SpeechClientBuilder { ChannelCredentials = credential.ToChannelCredentials() }.Build();
                    var response = speech.Recognize(new RecognitionConfig()
                    {
                        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRateHertz = 16000,
                        LanguageCode = "tr",

                    }, RecognitionAudio.FromFile(output));

                    txtSentence.Clear();

                    foreach (var result in response.Results)
                    {
                        foreach (var alternative in result.Alternatives)
                        {
                            txtSentence.Text = alternative.Transcript;
                        }
                    }
                    if (txtSentence.Text.Length == 0)
                    {
                        MessageBox.Show("Audio recording too long or no audio detected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        File.Delete(output);
                    }
                    else
                    {
                        await PlayButtonAsync(txtSentence.Text);
                    }
                    SpeechToTextAfter();
                }
                else
                {
                    MessageBox.Show("Please Check Your Internet Connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SpeechToTextAfter();
                }
            }
            else
            {
                txtSentence.Text = "No Audio File Found";
                SpeechToTextAfter();
            }
        }
        private void SpeechToTextAfter()
        {
            btnRecord.Enabled = true;
            this.Cursor = Cursors.Default;
            waveIn = new WaveIn();
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(WaveIn_DataAvailable);
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
        }


        #endregion

        private async Task PlayButtonAsync(string sentence)
        {
            if (sentence == "Please enter some text or record audio!" || sentence == "" || sentence == "We are listening to you")
            {
                MessageBox.Show("Please enter valid text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                btnConvert.Enabled = false;
                List<string> wordsList = await wordFunction.SplitWord(sentence.ToLower(new CultureInfo("tr-TR", false)));
                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
                PlayVideonScreen(wordsList);
            }
        }

        private void txtSentence_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtSentence.Text == "Please enter some text or record audio!" ||
                txtSentence.Text == "Audio recording too long or no audio detected." ||
                txtSentence.Text == "No Audio File Found" || txtSentence.Text == "We are listening to you")
                txtSentence.Clear();
        }
    }
}
