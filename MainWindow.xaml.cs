using NAudio.Wave;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VoiceClient
{
    public partial class MainWindow : Window
    {
        private WaveInEvent waveSource = null;
        private WaveFileWriter waveFile = null;
        private string outputFileName;

        private DispatcherTimer timer = new DispatcherTimer(); // Таймер для отсчета времени записи
        private TimeSpan recordingTime; // Время записи

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1); // Срабатывание каждую секунду
            timer.Tick += Timer_Tick;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (waveSource == null)
                {
                    outputFileName = "audio#" + Guid.NewGuid().ToString();
                    waveSource = new WaveInEvent();
                    waveSource.WaveFormat = new WaveFormat(44100, 1);
                    waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                    waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

                    waveFile = new WaveFileWriter(outputFileName, waveSource.WaveFormat);

                    recordingTime = TimeSpan.Zero;
                    timerTextBlock.Text = recordingTime.ToString(@"hh\:mm\:ss");
                    timer.Start();
                    waveSource.StartRecording();
                }
                else
                {
                    MessageBox.Show("Запись уже в процессе.");
                }
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show(naudioex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                if (waveFile != null)
                {
                    waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                    waveFile.Flush();
                }
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show(naudioex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (waveSource != null)
                {
                    timer.Stop();
                    waveSource.StopRecording();
                }
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show(naudioex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            recordingTime = recordingTime.Add(TimeSpan.FromSeconds(1));
            timerTextBlock.Text = recordingTime.ToString(@"hh\:mm\:ss");
        }

        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            try
            {
                if (waveSource != null)
                {
                    waveSource.Dispose();
                    waveSource = null;
                }

                if (waveFile != null)
                {
                    waveFile.Dispose();
                    waveFile = null;
                }
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show(naudioex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WaveFileReader fileReader = new WaveFileReader(outputFileName);
                WaveOutEvent waveOut = new WaveOutEvent();
                waveOut.Init(fileReader);
                do
                {
                   waveOut.Play();
                }
                while (waveOut.PlaybackState == PlaybackState.Playing);

                MessageBox.Show("Воспроизведение завершено.");
                waveOut.Dispose();
                waveOut = null;
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show(naudioex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}