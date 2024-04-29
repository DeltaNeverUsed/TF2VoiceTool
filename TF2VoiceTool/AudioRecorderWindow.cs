using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Utils;
using NAudio.Wave;
using Terminal.Gui;

namespace TF2VoiceTool;

public class AudioRecorderWindow : Window
{
    public WaveInEvent? AudioIn;
    public WaveOutEvent? AudioOut;

    private ProgressBar _progressBar;
    private WaveFileWriter _writer;

    private MemoryStream _memoryStream = new MemoryStream();

    private RawSourceWaveStream _stream;
    
    public AudioRecorderWindow()
    {
        Title = "Audio Recorder: "+WaveInEvent.GetCapabilities(0).ProductName;

        _progressBar = new ProgressBar()
        {
            Width = Dim.Fill()
        };
        
        Add(_progressBar);
        
        Application.MainLoop.AddIdle(UpdateProgress);
        
        AudioIn = new WaveInEvent();
        AudioIn.DeviceNumber = 0; // Change this number to select a different input device
        AudioIn.WaveFormat = new WaveFormat(44100, 16, 1); // Adjust the format as needed
        
        AudioIn.DataAvailable += (sender, e) =>
        {
            _memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
        };
    }

    ~AudioRecorderWindow()
    {
        AudioOut?.Stop();
        AudioOut?.Dispose();
        AudioIn?.Dispose();

        Application.MainLoop.RemoveIdle(UpdateProgress);
    }

    private bool _isRecording;

    public void StartRecording()
    {
        if (_isRecording)
        {
            StopRecording();
            return;
        }
        
        _memoryStream = new MemoryStream();
        AudioIn?.StartRecording();

        _isRecording = true;
        
        MainWindow.Main.Debug.PrintLine("Started Recording!");
    }

    public void StopRecording()
    {
        AudioIn?.StopRecording();
        _isRecording = false;
        Play();
        MainWindow.Main.Debug.PrintLine("Stopped Recording!");
    }

    public void Play()
    {
        if (AudioIn == null)
        {
            MainWindow.Main.Debug.PrintLine("Couldn't play recording! AudioIn was null!");
            return;
        }
        if (_memoryStream.Length < 10)
        {
            MainWindow.Main.Debug.PrintLine("MemoryStream was Empty!");
            return;
        }
        
        _stream = new RawSourceWaveStream(_memoryStream, AudioIn.WaveFormat);
        MainWindow.Main.Debug.PrintLine($"Audio stream is {_stream.TotalTime.TotalSeconds}s long");
        
        Play(_stream);
    }

    public void Play(RawSourceWaveStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        AudioOut = new WaveOutEvent();
        AudioOut.Init(stream);
        AudioOut.Play();
    }
    
    public void Stop()
    {
        AudioOut?.Stop();
    }

    public void Save(string filePath)
    {
        if (AudioIn == null)
        {
            MainWindow.Main.Debug.PrintLine("Couldn't save recording! AudioIn was null!");
            return;
        }

        var folder = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(folder))
            if (folder != null)
                Directory.CreateDirectory(folder);

        var writer = new LameMP3FileWriter(filePath, AudioIn.WaveFormat, 128);
        _stream.Seek(0, SeekOrigin.Begin);
        
        _stream.CopyTo(writer);
        writer.Close();
        
        MainWindow.Main.Debug.PrintLine("Saved recording to: " + filePath);
    }

    private bool UpdateProgress()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_stream == null || AudioOut == null)
            return true;
        
        _progressBar.Fraction = (float)(AudioOut.GetPositionTimeSpan().TotalSeconds / _stream.TotalTime.TotalSeconds);
        _progressBar.Draw();
        
        return true;
    }
}