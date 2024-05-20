using NAudio.Wave;
using System;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;

namespace VoiceClient
{
    public partial class MainWindow : Window
    {
        AudioClass AC = new AudioClass();

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
            timer.Start();

            AC.StartRecordAudio();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            AC.StopRecord();
            UpdateListView();

            if (File.Exists(AC.OutputFileName))
            {
                byte[] audioBytes = AC.GetRecordedAudioBytes(AC.OutputFileName);
                MessageBox.Show($"Запись остановлена. Размер массива байтов: {audioBytes?.Length ?? 0}");
            }
            else
            {
                MessageBox.Show($"Файл {AC.OutputFileName} не найден.");
            }
        }
            private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            AC.PlayAudio();
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listView.SelectedItem != null)
                {
                    dynamic selectedFile = listView.SelectedItem; // Получение выбранного элемента из ListView
                    string selectedFileName = selectedFile.FileName; // Получение имени выбранного файла
                    string filePathToPlay = AC.WaveFiles.Find(wf => wf.Filename == selectedFileName)?.Filename; // Поиск соответствующего пути к файлу
                    if (filePathToPlay != null)
                        AC.OutputFileName = filePathToPlay;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateListView()
        {
            try
            {
                listView.Items.Clear();

                foreach (WaveFileWriter file in AC.WaveFiles)
                {
                    listView.Items.Add(new
                    {
                        FileName = file.Filename,
                        Duration = AC.GetFileDuration(file) // Метод для получения длительности файла
                    });
                }
            }catch(Exception ex)
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