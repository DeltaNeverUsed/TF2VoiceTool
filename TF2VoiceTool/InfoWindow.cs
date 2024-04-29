using Terminal.Gui;

namespace TF2VoiceTool;

public class InfoWindow : Window
{
    public string CurrentFile;

    private Label _currentFileLabel;
    private Label _controlsLabel;
    
    public InfoWindow()
    {
        Title = "InfoWindow";

        _currentFileLabel = new Label()
        {
            Width = Dim.Fill()
        };

        _controlsLabel = new Label()
        {
            Width = Dim.Fill(),
            Y = 3,
                
            
            Text = "N: Next audio file\n" +
                   "E: Replay original audio\n" +
                   "R: Replay recording\n" +
                   "Z: Start recording\n" +
                   "X: Stop recording\n" +
                   "S: Save current recording",
        };
        
        Add(_currentFileLabel, _controlsLabel);
    }

    public void Update()
    {
        _currentFileLabel.Text = "Dubbing: " + CurrentFile;
        
        Draw();
    }
}