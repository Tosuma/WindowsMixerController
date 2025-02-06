using AudioMixer.DataStructure;
using AudioMixer.Helpers;

namespace AudioMixer.Views;
internal class EditSessionView : IView
{
    private ProcessList _processes;
    private GroupDictionary _groups;
    private Navigator _navigator;
    private int _selectedIndex;
    private int _editElementIndex = 0;

    public EditSessionView(ProcessList processes, GroupDictionary groups, Navigator navigator, int selectedIndex)
    {
        _processes = processes;
        _groups = groups;
        _navigator = navigator;
        _selectedIndex = selectedIndex;
    }

    private MenuItem CurrentProcess => _processes[_selectedIndex];

    public IView? HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                if (_editElementIndex == 0)
                    return _navigator.AdjustingVolumeView(_selectedIndex);
                else
                    CurrentProcess.Muted = !CurrentProcess.Muted;

                break;

            case ConsoleKey.Escape:
                return _navigator.MainView(_selectedIndex);

            case ConsoleKey.RightArrow:
                _editElementIndex = 1;
                break;

            case ConsoleKey.LeftArrow:
                _editElementIndex = 0;
                break;

            case ConsoleKey.UpArrow:
                if (_editElementIndex == 0)
                    CurrentProcess.IncreaseVolume();
                break;

            case ConsoleKey.DownArrow:
                if (_editElementIndex == 0)
                    CurrentProcess.DecreaseVolume();
                break;
        }
        return this;
    }

    public void Render()
    {
        KeyBinds keyAndDescription = new()
        {
            ["[↑]"] = "Increase volume",
            ["[→]"] = "Move marker right",
            ["[↓]"] = "Decrease volume",
            ["[←]"] = "Move marker left",
            ["[Enter]"] = "Edit volume or flip mute",
            ["[Esc]"] = "Exit edit",
        };

        RenderHelper.WriteKeyOptions(keyAndDescription, 2);


        string processTitle = "Process name";
        int longestSessionTitle = Math.Max(
            processTitle.Length,
            _processes.Select(p => p.Name)
                      .DefaultIfEmpty("")
                      .Max(s => s.Length));

        Console.WriteLine($"+-{new string('-', longestSessionTitle)}-+--------+-------+");
        Console.WriteLine($"| {processTitle.PadRight(longestSessionTitle)} | Volume | Muted |");
        Console.WriteLine($"+-{new string('-', longestSessionTitle)}-+--------+-------+");

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        for (int i = 0; i < _processes.Count; i++)
        {
            bool isCursorLocation = i == _selectedIndex;
            var name = _processes[i].Name;
            var volume = $"{_processes[i].GetVolume(),4:0.##}% ";
            var muted = $"{_processes[i].Muted,-5}";
            var marker = i == _selectedIndex ? "* " : "| ";

            Console.Write($"{marker}{name.PadRight(longestSessionTitle)} |");

            if (_editElementIndex == 0 && isCursorLocation)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" {volume} ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"| {muted} |");
            }
            else if (_editElementIndex == 1 && isCursorLocation)
            {
                Console.Write($" {volume} |");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" {muted} ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("|");
            }
            else
            {
                Console.WriteLine($" {volume} | {muted} |");
            }
        }
    }
}
