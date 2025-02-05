using AudioMixer.DataStructure;
using AudioMixer.Helpers;
using AudioMixer.Views;

namespace AudioMixer;
public class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        var processes = AudioSessionHelper.GetProcessesForMenu();
        var groups = GroupHelper.LoadConfiguration("config.json");

        GroupHelper.ApplyGroupConfigs(processes, groups);
        
        var navigator = new Navigator(processes, groups);

        IView currentView = navigator.MainView();

        bool running = true;
        while (running)
        {
            Console.Clear();
            currentView.Render();

            ConsoleKey keyPress = Console.ReadKey(true).Key;
            IView? nextView = currentView.HandleInput(keyPress);

            if (nextView is null)
                running = false;
            else
                currentView = nextView;
        }


        //MainView.Run(processes, groups);

        GroupHelper.SaveConfiguration("config.json", groups);
    }
}
