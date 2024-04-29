using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using Terminal.Gui;

namespace TF2VoiceTool;

public class AudioPlayerWindow : Window
{
    private Mp3FileReader? _audioStream;

    public WaveOutEvent? AudioOut;

    private ProgressBar _progressBar;

    private string _currentFile;
    
    public AudioPlayerWindow()
    {
        Title = "Audio Playback";

        _progressBar = new ProgressBar()
        {
            Width = Dim.Fill()
        };
        
        Add(_progressBar);
        
        Application.MainLoop.AddIdle(UpdateProgress);
    }

    public void LoadPath(string filePath)
    {
        _currentFile = filePath;
        
        var fileInfo = new FileInfo(filePath);
        Title = "Playing: "+fileInfo.Name;
        
        MainWindow.Main.Debug.PrintLine("Audio Player loading: "+fileInfo.Name);
        
        AudioOut?.Dispose();
        AudioOut = new WaveOutEvent();
        
        _audioStream = new Mp3FileReader(filePath);
        AudioOut.Init(_audioStream);
    }

    ~AudioPlayerWindow()
    {
        AudioOut?.Stop();
        AudioOut?.Dispose();
        _audioStream?.Dispose();

        Application.MainLoop.RemoveIdle(UpdateProgress);
    }

    public void Play(string filePath)
    {
        _currentFile = filePath;
        Play();
    }

    public void Play()
    {
        LoadPath(_currentFile);
        AudioOut?.Play();
    }
    
    public void Stop()
    {
        AudioOut?.Stop();
    }

    private bool UpdateProgress()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_audioStream == null)
            return true;
        
        _progressBar.Fraction = (float)(AudioOut.GetPositionTimeSpan().TotalSeconds / _audioStream.TotalTime.TotalSeconds);
        _progressBar.Draw();
        
        return true;
    }
}