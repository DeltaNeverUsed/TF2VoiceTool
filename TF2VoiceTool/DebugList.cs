using System.Text;
using System.Text.RegularExpressions;
using Terminal.Gui;

namespace TF2VoiceTool;

public class DebugList : ListView
{
    private List<string> _list = new List<string>();
    
    public static List<string> GetChunks(string value, int chunkSize)
    {
        List<string> triplets = new List<string>();
        while (value.Length > chunkSize)
        {
            triplets.Add(value.Substring(0, chunkSize));
            value = value.Substring(chunkSize);
        }
        if (value != "")
            triplets.Add(value);
        return triplets;
    }
    
    public DebugList()
    {
        this.SetSource(_list);

        CanFocus = false;
    }

    public void PrintLine(string msg)
    {
        var chunks = GetChunks(msg, Bounds.Size.Width);

        foreach (var chunk in chunks)
        {
            _list.Add(chunk);
        }
        
        var len = _list.Count;
        SelectedItem = len - 1;
    }
}