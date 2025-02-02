using AudioMixer.DataStructure;
using AudioMixer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.Menus;
internal static class EditMenu
{
    public static List<MenuItem> Run(List<MenuItem> processes, int selectedIndex)
    {
        int editElementIndex = 0;
        bool running = true;

        while (running)
        {
            Console.Clear();

            Render(processes, selectedIndex, editElementIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            (var currentState, editElementIndex, processes[selectedIndex]) = HandleInput(keyInfo.Key, editElementIndex, processes[selectedIndex]);

            switch (currentState)
            {
                case MenuState.MainMenu:
                    running = false;
                    break;

                case MenuState.AdjustingVolume:
                    processes = AdjustVolumeMenu.Run(processes, selectedIndex);
                    break;

                default:
                    break;
            }
        }

        return processes;
    }

    public static (MenuState currentState, int editElementIndex, MenuItem process) HandleInput(ConsoleKey key, int editElementIndex, MenuItem process)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                if (editElementIndex == 0)
                    return (MenuState.AdjustingVolume, editElementIndex, process);
                else
                    process.Muted = !process.Muted;

                break;

            case ConsoleKey.Escape:
                return (MenuState.MainMenu, 0, process);

            case ConsoleKey.RightArrow:
                editElementIndex = 1;
                break;

            case ConsoleKey.LeftArrow:
                editElementIndex = 0;
                break;

            case ConsoleKey.UpArrow:
                process.IncreaseVolume();
                break;

            case ConsoleKey.DownArrow:
                process.DecreaseVolume();
                break;
        }
        return (MenuState.EditMenuItem, editElementIndex, process);
    }

    public static void Render(List<MenuItem> processes, int selectedIndex, int editElementIndex)
    {
        string processTitle = "Process name";
        int longestTitle = Math.Max(processTitle.Length, processes.Max(p => p.Name.Length));

        Console.WriteLine("Use Left/Right to switch between Volume and Muted. Esc to return.\n");
        Console.WriteLine($"| {processTitle.PadRight(longestTitle)} | Volume | Muted |");
        Console.WriteLine($"+-{new string('-', longestTitle)}-+--------+-------+");

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        for (int i = 0; i < processes.Count; i++)
        {
            bool isCursorLocation = i == selectedIndex;
            var name = processes[i].Name;
            var volume = $"{processes[i].GetVolume(),4:0.##}% ";
            var muted = $"{processes[i].Muted,-5}";
            var marker = i == selectedIndex ? "* " : "| ";

            Console.Write($"{marker}{name.PadRight(longestTitle)} |");

            if (editElementIndex == 0 && isCursorLocation)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" {volume} ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"| {muted} |");
            }
            else if (editElementIndex == 1 && isCursorLocation)
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
