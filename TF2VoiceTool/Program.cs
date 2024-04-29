// See https://aka.ms/new-console-template for more information

using Terminal.Gui;
using TF2VoiceTool;


Application.Run<MainWindow> ();

// Before the application exits, reset Terminal.Gui for clean shutdown
Application.Shutdown ();

// Defines a top-level window with border and title

public class MainWindow : Window
{
    public static MainWindow? Main;

    public DebugList Debug;
    private string _sourceAudioFolder;
    private string _outputAudioFolder;

    private List<string> _todoFileNames;

    private bool _doneGetting;
    
    private AudioPlayerWindow _audioPlayerWindow;
    private AudioRecorderWindow _audioRecorderWindow;
    private InfoWindow _info;
    
    public MainWindow ()
    {
        Main = this;
        
        Title = "TF2 Voice Tool (Ctrl+Q to quit)";

        var debugWindow = new Window()
        {
            Title = "Debug output",
            Width = Dim.Percent(100f),
            Height = Dim.Percent(50f),
            
            X = Pos.Percent(0f),
            Y = Pos.Percent(50f)
        };

        Debug = new DebugList()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        
        _audioPlayerWindow = new AudioPlayerWindow()
        {
            X = 0,
            Y = 0,
            
            Width = Dim.Percent(50f),
            Height = 3,
        };
        _audioRecorderWindow = new AudioRecorderWindow()
        {
            X = 0,
            Y = 3,
            
            Width = Dim.Percent(50f),
            Height = 3,
        };

        _info = new InfoWindow()
        {
            Width = Dim.Percent(50f),
            Height = Dim.Percent(50f),
            
            X = Pos.Percent(50f),
            Y = Pos.Percent(0f)
        };
        
        debugWindow.Add(Debug);
        
        Add(debugWindow, _audioPlayerWindow, _audioRecorderWindow, _info);
        
    }
    
    public void NextAudio()
    {
        var first = _todoFileNames.First();
        _info.CurrentFile = first;
        _info.Update();
        
        _audioPlayerWindow.Play(_sourceAudioFolder + "\\" + first);

        _todoFileNames.Remove(first);
    }

    public override bool OnKeyDown(KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.n:
                NextAudio();
                break;
            case Key.e:
                _audioPlayerWindow.Play();
                break;
            case Key.r:
                _audioRecorderWindow.Play();
                break;
            case Key.z:
                _audioRecorderWindow.StartRecording();
                break;
            case Key.x:
                _audioRecorderWindow.StopRecording();
                break;
            case Key.s:
                _audioRecorderWindow.Save(_outputAudioFolder + "\\" + _info.CurrentFile);
                NextAudio();
                break;
        }


        return base.OnKeyDown(keyEvent);
    }

    private void GetVariables()
    {
        
        selectSource:
        
        var sourceAudioFiles = new OpenDialog("Please Select Source Audio Folder", openMode: OpenMode.Directory)
        {
            AllowsMultipleSelection = false,
        };
        
        Application.Run(sourceAudioFiles);
        
        if (!sourceAudioFiles.Canceled && sourceAudioFiles.FilePaths.Count > 0) {
            _sourceAudioFolder = sourceAudioFiles.FilePaths [0];
            Debug.PrintLine("Source audio folder selected: " + _sourceAudioFolder);
        }
        else
        {
            Debug.PrintLine("No source folder selected!");
            goto selectSource;
        }
        
        selectOutput:
        var startFolder = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Team Fortress 2\\tf\\custom";
        if (!Directory.Exists(startFolder))
            startFolder = "";
        var outputAudioFiles = new OpenDialog("Please Select TF2 custom sound folder", openMode: OpenMode.Directory)
        {
            AllowsMultipleSelection = false,
            Path = startFolder
        };
        
        Application.Run(outputAudioFiles);
        
        if (!outputAudioFiles.Canceled && outputAudioFiles.FilePaths.Count > 0) {
            _outputAudioFolder = outputAudioFiles.FilePaths [0];
            Debug.PrintLine("Ouput audio folder selected: " + _outputAudioFolder);
        }
        else
        {
            Debug.PrintLine("No output folder selected!");
            goto selectOutput;
        }


        _todoFileNames = Directory.EnumerateFiles(_sourceAudioFolder, "*.mp3", SearchOption.AllDirectories).ToList();
        var alreadyDone = Directory.EnumerateFiles(_outputAudioFolder, "*.mp3", SearchOption.AllDirectories).ToList();

        var dirInfo = new DirectoryInfo(_sourceAudioFolder);

        for (var index = 0; index < _todoFileNames.Count; index++)
            _todoFileNames[index] = _todoFileNames[index][(dirInfo.FullName.Length + 1)..];

        dirInfo = new DirectoryInfo(_outputAudioFolder);
        
        for (var index = 0; index < alreadyDone.Count; index++)
            alreadyDone[index] = alreadyDone[index][(dirInfo.FullName.Length + 1)..];
        
        _todoFileNames = _todoFileNames.Where(x => !alreadyDone.Contains(x)).ToList();

        
        Debug.Draw();
        _doneGetting = true;
    }

    public override void OnLoaded()
    {
        base.OnLoaded();

        GetVariables();
    }
}