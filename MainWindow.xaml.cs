using NAudio.Wave;
using System;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VoiceClient
{
    public partial class MainWindow : Window
    {
        private WaveInEvent waveSource = null;
        private WaveFileWriter waveFile = null;
        private List<WaveFileWriter> waveFiles = new List<WaveFileWriter>();
        private string outputFileName;

        private DispatcherTimer timer = new DispatcherTimer();
        private TimeSpan recordingTime;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(1); // Срабатывание каждую секунду
            timer.Tick += Timer_Tick;
            recordingTime = TimeSpan.Zero;
            timerTextBlock.Text = recordingTime.ToString(@"hh\:mm\:ss");
            StartRecordAudio();
            timer.Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            StopRecord();
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayAudio();
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listView.SelectedItem != null)
                {
                    dynamic selectedFile = listView.SelectedItem; // Получение выбранного элемента из ListView
                    string selectedFileName = selectedFile.FileName; // Получение имени выбранного файла
                    string filePathToPlay = waveFiles.Find(wf => wf.Filename == selectedFileName)?.Filename; // Поиск соответствующего пути к файлу
                    if (filePathToPlay != null)
                        outputFileName = filePathToPlay;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task UpdateListView()
        {
            listView.Items.Clear();

            foreach (WaveFileWriter file in waveFiles)
            {
                listView.Items.Add(new
                {
                    FileName = file.Filename,
                    Duration = GetFileDuration(file.Filename) // Метод для получения длительности файла
                });
            }
        }

        private string GetFileDuration(string fileName)
        {
            // Здесь можно добавить логику для определения длительности записанного файла
            // Например, использовать NAudio или другие библиотеки для работы с аудио
            // Возвратим пока что просто "N/A"
            return "N/A";
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

        private void StartRecordAudio()
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
                    waveFiles.Add(waveFile);

                    waveSource.StartRecording();

                    UpdateListView();
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

        private void StopRecord()
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

        private void PlayAudio()
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            recordingTime = recordingTime.Add(TimeSpan.FromSeconds(1));
            timerTextBlock.Text = recordingTime.ToString(@"hh\:mm\:ss");
        }
    }
}