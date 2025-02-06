using AudioMixer.DataStructure;
using AudioMixer.Helpers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace AudioMixer.Views;
internal class MainView : IView
{
    private ProcessList _processes;
    private GroupDictionary _groups;
    private Navigator _navigator;
    private int _selectedIndex;

    public MainView(ProcessList processes, GroupDictionary groups, Navigator navigator, int selectedIndex)
    {
        _processes = processes;
        _groups = groups;
        _navigator = navigator;
        _selectedIndex = selectedIndex;
    }

    public IView? HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Escape:
                return null;

            case ConsoleKey.Enter:
                return _navigator.EditSessionView(_selectedIndex);

            case ConsoleKey.UpArrow:
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
                break;

            case ConsoleKey.DownArrow:
                _selectedIndex = Math.Min(_processes.Count - 1, _selectedIndex + 1);
                break;

            case ConsoleKey.G:
                return _navigator.GroupView();

            case ConsoleKey.R:
                _navigator.UpdateProcesses(AudioSessionHelper.GetProcessesForMenu());
                return _navigator.MainView();

            case ConsoleKey.A:
                GroupHelper.ApplyGroupConfigs(_processes, _groups);
                break;

            case ConsoleKey.S:
                GroupHelper.SaveConfiguration("config.json", _groups);
                break;
        }

        return this;
    }

    public void Render()
    {
        KeyBinds keyAndDescription = new()
        {
            ["[↑]"] = "Move marker up",
            ["[↓]"] = "Move marker down",
            ["[Enter]"] = "Edit",
            ["[Esc]"] = "Quit",
            ["[R]"] = "Refresh sessions",
            ["[G]"] = "Group settings",
            ["[A]"] = "Apply group configs",
            ["[S]"] = "Save current group config",
        };

        RenderHelper.WriteKeyOptions(keyAndDescription, 2);

        string processTitle = "Process name";
        int longestSessionTitle = Math.Max(
            processTitle.Length, 
            _processes.Select(p => p.Name)
                      .DefaultIfEmpty("")
                      .Max(s => s.Length));

        string groupTitle = "Group";
        int longestGroupTitle = Math.Max(groupTitle.Length, _groups.Keys.DefaultIfEmpty("").Max(k => k.Length));

        Console.WriteLine($"+-{new string('-', longestSessionTitle)}-+--------+-------+-------+");
        Console.WriteLine($"| {processTitle.PadRight(longestSessionTitle)} | Volume | Muted | {groupTitle.PadRight(longestGroupTitle)} |");
        Console.WriteLine($"+-{new string('-', longestSessionTitle)}-+--------+-------+-------+");

        for (int i = 0; i < _processes.Count; i++)
        {
            var name = _processes[i].Name;
            var volume = _processes[i].GetVolume();
            var muted = _processes[i].Muted;
            var marker = i == _selectedIndex ? "> " : "| ";

            var inGroup = _groups.GetGroupWithSession(name);

            Console.WriteLine($"{marker}{name.PadRight(longestSessionTitle)} | {volume,4:0.##}%  | {muted,-5} | {inGroup.PadRight(longestGroupTitle)} |");
        }
    }
}
