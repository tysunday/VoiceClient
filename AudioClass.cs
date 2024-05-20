using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceClient
{
    public class AudioClass
    {
        private WaveInEvent waveSource = null;
        private WaveFileWriter waveFile = null;
        public string GetFileDuration(WaveFileWriter waveFile)
        {
            try
            {
                if (waveFile != null)
                {
                    waveFile.Dispose(); // здесь освобождаем ресурсы чтобы им можно было воспользоваться далее или что-то типа такого

                    using (WaveFileReader reader = new WaveFileReader(waveFile.Filename))
                    {
                        TimeSpan duration = reader.TotalTime;
                        return duration.ToString(@"hh\:mm\:ss");
                    }
                }
                return "N/A";
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show($"Ошибка в MmException: {naudioex.Message}");
                return "N/A";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в Exception: {ex.Message}");
                return "N/A";
            }
        }

        public void StartRecordAudio(string outputFileName, List<WaveFileWriter> WaveFiles = null)
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

                    if (WaveFiles != null)
                        WaveFiles.Add(waveFile);

                    waveSource.StartRecording();
                }
                else
                {
                    MessageBox.Show("Запись уже в процессе.");
                }
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show($"Ошибка в MmException: {naudioex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в Exception: {ex.Message}");
            }
        }

        public void StopRecord()
        {
            try
            {
                if (waveSource != null)
                {
                    waveSource.StopRecording();
                }
            }
            catch (NAudio.MmException naudioex)
            {
                MessageBox.Show($"Ошибка в MmException: {naudioex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в Exception: {ex.Message}");
            }
        }

        public void PlayAudio(string outputFileName)
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
                MessageBox.Show($"Ошибка в MmException: {naudioex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в Exception: {ex.Message}");
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
                MessageBox.Show($"Ошибка в MmException: {naudioex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в Exception: {ex.Message}");
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
                MessageBox.Show($"Ошибка в MmException: {naudioex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в Exception: {ex.Message}");
            }
        }
    }
}