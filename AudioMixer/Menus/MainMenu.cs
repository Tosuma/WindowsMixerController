using AudioMixer.DataStructure;
using AudioMixer.Handlers;
using AudioMixer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.Menus;
internal static class MainMenu
{
    public static void Run(List<MenuItem> processes, Dictionary<string, GroupInfo> groups)
    {
        int selectedIndex = 0;
        bool running = true;
        while (running)
        {
            Console.Clear();
            
            RenderMainMenu(processes, selectedIndex, groups);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            
            (var currentState, running, selectedIndex) = HandleMainMenuInput(keyInfo.Key, selectedIndex, processes.Count);

            switch (currentState)
            {
                case MenuState.EditMenuItem:
                    processes = EditMenu.Run(processes, selectedIndex);
                    break;

                case MenuState.GroupMenu:
                    GroupMenu.Run(processes, groups);
                    break;

            }
        }
    }

    public static (MenuState currentState, bool running, int selectedIndex) HandleMainMenuInput(ConsoleKey key, int selectedIndex, int numOfProcesses)
    {
        switch (key)
        {
            case ConsoleKey.Escape:
                return (MenuState.MainMenu, running: false, selectedIndex);

            case ConsoleKey.Enter:
                return (MenuState.EditMenuItem, running: true, selectedIndex);

            case ConsoleKey.UpArrow:
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = 0;
                break;

            case ConsoleKey.DownArrow:
                selectedIndex++;
                if (!(selectedIndex < numOfProcesses))
                    selectedIndex = numOfProcesses - 1;
                break;

            case ConsoleKey.G:
                return (MenuState.GroupMenu, running: true, selectedIndex);
        }

        return (MenuState.MainMenu, running: true, selectedIndex);
    }

    public static void RenderMainMenu(List<MenuItem> processes, int selectedIndex, Dictionary<string, GroupInfo> groups)
    {
        Console.WriteLine("Use Up/Down Arrow to select an item. Press Enter to edit, Esc to quit.\n");

        string processTitle = "Process name";
        int longestSessionTitle = Math.Max(processTitle.Length, processes.Max(p => p.Name.Length));
        string groupTitle = "Group";
        int longestGroupTitle = Math.Max(groupTitle.Length, groups.Keys.Max(k => k.Length));

        Console.WriteLine($"| {processTitle.PadRight(longestSessionTitle)} | Volume | Muted | {groupTitle.PadRight(longestGroupTitle)} |");
        Console.WriteLine($"+-{new string('-', longestSessionTitle)}-+--------+-------+-------+");

        for (int i = 0; i < processes.Count; i++)
        {
            var name = processes[i].Name;
            var volume = processes[i].GetVolume();
            var muted = processes[i].Muted;
            var marker = i == selectedIndex ? "> " : "| ";

            var inGroup = groups.GetGroupWithSession(name);

            Console.WriteLine($"{marker}{name.PadRight(longestSessionTitle)} | {volume,4:0.##}%  | {muted,-5} | {inGroup.PadRight(longestGroupTitle)} |");

        }
    }
}
