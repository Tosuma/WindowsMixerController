using AudioMixer.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.Menus;
internal class EditGroup
{
    internal static void Run(List<MenuItem> processes, Dictionary<string, GroupInfo> groups, int selectedIndex)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();

            Render(processes, groups, selectedIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            var currentState = HandleInput(keyInfo.Key);

            switch (currentState)
            {
                case MenuState.GroupMenu:
                    running = false;
                    break;
            }
        }
    }

    private static void Render(List<MenuItem> processes, Dictionary<string, GroupInfo> groups, int selectedIndex)
    {

    }
    private static MenuState HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.Escape:
                return MenuState.GroupMenu;
        }

        return MenuState.EditGroupItem;
    }
}
