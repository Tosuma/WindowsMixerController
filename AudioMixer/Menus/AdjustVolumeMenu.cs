using AudioMixer.DataStructure;
using AudioMixer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.Menus;
internal static class AdjustVolumeMenu
{
    public static List<MenuItem> Run(List<MenuItem> processes, int selectedIndex)
    {
        bool running = true;
        float oldVolume = processes[selectedIndex].GetVolume();

        while (running)
        {
            Console.Clear();

            Render(processes, selectedIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            (var currentState, bool cancelVolumeChange) = HandleInput(keyInfo.Key, processes[selectedIndex]);

            switch (currentState)
            {
                case MenuState.EditMenuItem:
                    running = false;
                    if (cancelVolumeChange)
                        processes[selectedIndex].SetVolume(oldVolume);
                    break;

                default:
                    break;
            }
        }
        
        return processes;
    }

    public static (MenuState currentState, bool cancelVolumeChange) HandleInput(ConsoleKey key, MenuItem process)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                return (MenuState.EditMenuItem, cancelVolumeChange: false);

            case ConsoleKey.Escape:
                return (MenuState.EditMenuItem, cancelVolumeChange: true);

            case ConsoleKey.DownArrow:
            case ConsoleKey.LeftArrow:
                process.DecreaseVolume();
                break;

            case ConsoleKey.UpArrow:
            case ConsoleKey.RightArrow:
                process.IncreaseVolume();
                break;
        }

        return (MenuState.AdjustingVolume, cancelVolumeChange: false);
    }

    public static void Render(List<MenuItem> processes, int selectedIndex)
    {
        string processTitle = "Process name";
        int longestTitle = Math.Max(processTitle.Length, processes[selectedIndex].Name.Length);

        Console.WriteLine("Use Left/Right to decrease/increase volume. Esc to cancel changes, Enter to save changes.\n");
        Console.WriteLine($"| {processTitle.PadRight(longestTitle)} | Volume | Muted |");
        Console.WriteLine($"+-{new string('-', longestTitle)}-+--------+-------+");

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        var name = processes[selectedIndex].Name;
        var volume = $"{processes[selectedIndex].GetVolume(),4:0.##}% ";
        var muted = $"{processes[selectedIndex].Muted,-5}";


        Console.Write($"* {name.PadRight(longestTitle)} | {volume} | {muted} |");

        Console.WriteLine();
        Console.WriteLine();
        DrawVolumeBar((int)processes[selectedIndex].GetVolume(), 100, 32);
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
