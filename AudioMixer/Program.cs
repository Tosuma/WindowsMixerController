using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using AudioMixer.DataStructure;
using AudioMixer.Handlers;
using AudioMixer.Helpers;
using AudioMixer.Menus;
using NAudio.CoreAudioApi;

namespace AudioMixer;
public class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        //AudioMixer mixer = new AudioMixer();

        var processes = AudioSessionHelper.GetProcessesForMenu();
        var groups = GroupHelper.LoadConfiguration("config.json");
        GroupHelper.ApplyGroupConfigs(processes, groups);

        MainMenu.Run(processes, groups);

        GroupHelper.SaveConfiguration("config.json", groups);
    }
}