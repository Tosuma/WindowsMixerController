using AudioMixer.DataStructure;

namespace AudioMixer.Views;
internal class AdjustVolumeView : IView
{
    private ProcessList _processes;
    private GroupDictionary _groups;
    private int _selectedIndex;
    private Navigator _navigator;
    private float _originalVolume;

    public AdjustVolumeView(ProcessList processes, GroupDictionary groups, Navigator navigator, int selectedIndex)
    {
        _processes = processes;
        _groups = groups;
        _selectedIndex = selectedIndex;
        _navigator = navigator;
        _originalVolume = processes[selectedIndex].GetVolume();
    }

    private MenuItem CurrentProcess => _processes[_selectedIndex];

    public IView? HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                return _navigator.EditSessionView(_selectedIndex);

            case ConsoleKey.Escape:
                CurrentProcess.SetVolume(_originalVolume);
                return _navigator.EditSessionView(_selectedIndex);

            case ConsoleKey.DownArrow:
            case ConsoleKey.LeftArrow:
                CurrentProcess.DecreaseVolume();
                break;

            case ConsoleKey.UpArrow:
            case ConsoleKey.RightArrow:
                CurrentProcess.IncreaseVolume();
                break;
        }

        return this;
    }

    public void Render()
    {
        Console.WriteLine("Use Left/Right to decrease/increase volume. Esc to cancel changes, Enter to save changes.\n");
        string processTitle = "Process name";
        var process = CurrentProcess;
        int longestSessionTitle = Math.Max(
            processTitle.Length,
            _processes.Select(p => p.Name)
                      .DefaultIfEmpty("")
                      .Max(s => s.Length));

        Console.WriteLine($"| {processTitle.PadRight(longestSessionTitle)} | Volume | Muted |");
        Console.WriteLine($"+-{new string('-', longestSessionTitle)}-+--------+-------+");

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        var name = process.Name;
        var volume = $"{process.GetVolume(),4:0.##}% ";
        var muted = $"{process.Muted,-5}";


        Console.Write($"* {name.PadRight(longestSessionTitle)} | {volume} | {muted} |");

        Console.WriteLine();
        Console.WriteLine();
        DrawVolumeBar((int)process.GetVolume(), 100, 32);
    }

    private static void DrawVolumeBar(int progress, int total, int width = 50)
    {
        progress = progress > total ? total : progress;
        int filledWidth = (int)((double)progress / total * width);

        string filledBar = new string('█', filledWidth);
        string emptyBar = new string(' ', width - filledWidth);

        Console.Write($"\r[{filledBar + emptyBar}]");
    }
}
