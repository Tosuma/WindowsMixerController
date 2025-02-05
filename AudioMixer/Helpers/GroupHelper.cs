using AudioMixer.DataStructure;
using NAudio.CoreAudioApi;
using NAudio.Mixer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace AudioMixer.Helpers;
internal static class GroupHelper
{
    internal static void ApplyGroupConfigs(ProcessList processes, GroupDictionary groups)
    {
        foreach (var (_, groupInfo) in groups)
        {
            foreach (var sessionName in groupInfo.SessionNames)
            {
                processes
                    .Where(mi => Path.GetFileNameWithoutExtension(mi.FilePath).Equals(sessionName, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(mi => mi.SetVolume(groupInfo.Volume));
            }
        }
    }

    internal static GroupDictionary LoadConfiguration(string filePath)
    {
        var groups = new GroupDictionary();
        if (!File.Exists(filePath))
            return [];

        //Console.WriteLine("Loading configs...");
        try
        {
            string json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<GroupConfig>(json);
            if (config is null)
                return [];

            //Console.WriteLine($"Number of groups: {config.Groups.Count}");
            foreach (var (groupName, groupInfo) in config.Groups)
            {
                //Console.WriteLine($"\t'{groupName}' :: {groupInfo.SessionNames.Count}");
                groups.Add(groupName, groupInfo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
        }
        return groups;
    }

    internal static string GetGroupWithSession(this GroupDictionary groups, string sessionName)
    {
        foreach (var (groupName, groupInfo) in groups)
        {
            if (groupInfo.SessionNames.Contains(sessionName, StringComparer.OrdinalIgnoreCase))
                return groupName;
        }

        return string.Empty;
    }

    internal static void CreateGroup(this GroupDictionary groups)
    {

    }

    internal static void AddSessionsToGroup(this GroupDictionary groups, ProcessList sessions)
    {

    }

    internal static void RemoveSessionsFromGroup(this GroupDictionary groups, ProcessList sessions)
    {

    }

    internal static void IncreaseGroupVolume(this GroupDictionary groups, float volume)
    {

    }

    internal static void ChangeGroupMute(this GroupDictionary groups, bool mute)
    {

    }

    internal static void DeleteGroup(this GroupDictionary groups, string groupKey)
    {
        groups.Remove(groupKey);
    }

    internal static void SaveConfiguration(string v, GroupDictionary groups)
    {

    }
}
