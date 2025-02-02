
using AudioMixer.DataStructure;
using AudioMixer.Handlers;

namespace AudioMixer.Menus;

internal class GroupMenu
{
    internal static void Run(List<MenuItem> processes, Dictionary<string, GroupInfo> groups)
    {
        int selectedIndex = 0;
        bool running = true;
        while (running)
        {
            Console.Clear();
            Render(selectedIndex, processes, groups);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            (var currentState, selectedIndex) = HandleGroupInput(keyInfo.Key, selectedIndex, processes, groups);

            switch (currentState)
            {
                case MenuState.MainMenu:
                    running = false;
                    break;

                case MenuState.EditGroupItem:
                    EditGroup.Run(processes, groups, selectedIndex);
                    break;
            }
        }
    }

    private static (MenuState currentState, int selectedIndex) HandleGroupInput(ConsoleKey key, int selectedIndex, List<MenuItem> processes, Dictionary<string, GroupInfo> groups)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                return (MenuState.EditGroupItem, selectedIndex);

            case ConsoleKey.Escape:
                return (MenuState.MainMenu, selectedIndex);

            case ConsoleKey.UpArrow:
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = 0;
                break;

            case ConsoleKey.DownArrow:
                selectedIndex++;
                if (!(selectedIndex < groups.Count))
                    selectedIndex = groups.Count - 1;
                break;
        }
        return (MenuState.GroupMenu, selectedIndex);
    }

    private static void Render(int selectedIndex, List<MenuItem> processes, Dictionary<string, GroupInfo> groups)
    {
        int i = 0;
        Console.WriteLine("Use Up/Down Arrow to select an item. Press Enter to edit, Esc to quit.\n");
        Console.WriteLine($"{"Group name",-15} | Volume | Muted | Num sesssions |");
        foreach (var (groupName, groupInfo) in groups)
        {
            var name = (i == selectedIndex)
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