
using AudioMixer.DataStructure;
using AudioMixer.Helpers;

namespace AudioMixer.Views;

internal class GroupView : IView
{
    private ProcessList _processes;
    private GroupDictionary _groups;
    private Navigator _navigator;
    private int _selectedIndex;

    public GroupView(ProcessList processes, GroupDictionary groups, Navigator navigator, int selectedIndex)
    {
        _processes = processes;
        _groups = groups;
        _selectedIndex = selectedIndex;
        _navigator = navigator;
    }

    public IView? HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                return _navigator.EditGroupView(_selectedIndex);

            case ConsoleKey.Escape:
                return _navigator.MainView(_selectedIndex);

            case ConsoleKey.UpArrow:
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
                break;

            case ConsoleKey.DownArrow:
                _selectedIndex = Math.Min(_groups.Count - 1, _selectedIndex + 1);
                break;

            case ConsoleKey.R:
                _navigator.UpdateProcesses(AudioSessionHelper.GetProcessesForMenu());
                return _navigator.GroupView();

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
            ["[Esc]"] = "Go back",
            ["[R]"] = "Refresh sessions",
            ["[A]"] = "Apply group configs",
            ["[S]"] = "Save current group config",
        };

        RenderHelper.WriteKeyOptions(keyAndDescription, 2);


        int i = 0;
        Console.WriteLine($"{"Group name",-15} | Volume | Muted | Num sesssions |");
        foreach (var (groupName, groupInfo) in _groups)
        {
            var name = (i == _selectedIndex)
                ? $"* {groupName}"
                : $"  {groupName}";

            var volume = groupInfo.Volume;
            var muted = groupInfo.Muted;
            var numSessions = groupInfo.SessionNames.Count;

            Console.WriteLine($"{name,-15} | {volume,4:0.##}%  | {muted,-5} | {numSessions,6}");
            i++;
        }
    }
}