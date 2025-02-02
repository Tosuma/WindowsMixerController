using AudioMixer.Helpers;
using NAudio.CoreAudioApi;
using NAudio.Mixer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioMixer;
[Obsolete("This class is depricated and for an older implementation of the audio mixer", true)]
internal class AudioMixer
{
    private SessionCollection _sessions;

    // A dictionary of "group name" -> list of audio sessions
    private Dictionary<string, List<AudioSessionControl>> _groups
        = new Dictionary<string, List<AudioSessionControl>>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Expose this dictionary so ConfigManager can read/write group membership
    /// (by file path or process ID, etc.).
    /// </summary>
    public Dictionary<string, List<AudioSessionControl>> Groups => _groups;



    /// <summary>
    /// Constructs the AudioMixer and initializes the dictionary.
    /// </summary>
    public AudioMixer()
    {
        _groups = new Dictionary<string, List<AudioSessionControl>>(StringComparer.OrdinalIgnoreCase);
    }

    public int ActiveSessionCount => _sessions?.Count ?? 0;

    public AudioSessionControl GetSessionAtIndex(int index)
    {
        if (_sessions == null || index < 0 || index >= _sessions.Count)
            return null;
        return _sessions[index];
    }

    /// <summary>
    /// Main method to run the audio mixer logic.
    /// </summary>
    public void Run()
    {
        InitializeAudioDevices();
        ConfigManager.LoadConfiguration("config.json", this);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Active Audio Sessions:\n");

            // 1. List all sessions
            ListSessions();

            // 2. Prompt user: pick a session index, go to 'g' group menu, or 'q' to quit.
            Console.WriteLine("\nEnter a session index to modify, type 'g' for group menu, 's' for save config, or 'q' to quit:");
            var input = Console.ReadLine()?.Trim().ToLower();
                
            if (input == "q")
            {
                // Quit
                break;
            }
            else if (input == "g")
            {
                // Go to group management menu
                ManageGroups();
            }
            else if (input == "s")
            {
                ConfigManager.SaveConfiguration("config.json", this);
            }
            else
            {
                // Try to parse a session index
                if (!int.TryParse(input, out int sessionIndex) ||
                    sessionIndex < 0 || sessionIndex >= _sessions.Count)
                {
                    Console.WriteLine("Invalid choice. Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                // Show menu for that session (volume/mute)
                ShowSessionMenu(_sessions[sessionIndex]);
            }
        }
    }



    #region Core Audio Initialization

    private void InitializeAudioDevices()
    {
        var enumerator = new MMDeviceEnumerator();
        var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        var sessionManager = device.AudioSessionManager;
        _sessions = sessionManager.Sessions;

        // After sessions are ready, map any loaded config data by PID to actual sessions.
        ConfigManager.RemapGroupsToActiveSessions(this);
    }

    #endregion

    #region Session Listing & Manipulation

    private void ListSessions()
    {
        for (int i = 0; i < _sessions.Count; i++)
        {
            AudioSessionControl session = _sessions[i];
            float volumePercent = session.SimpleAudioVolume.Volume * 100;
            bool isMuted = session.SimpleAudioVolume.Mute;
            string displayName = AudioSessionHelper.GetSessionDisplayName(session);
            string filePath = AudioSessionHelper.GetProcessFilePath(session);

            Console.WriteLine($"{i}. {displayName} | Volume: {volumePercent:0.##}% | Muted: {isMuted}");
        }
    }

    private void ShowSessionMenu(AudioSessionControl session)
    {
        ShowSessionInfo(session);

        Console.WriteLine("What would you like to do?");
        Console.WriteLine("  [V] Change Volume");
        Console.WriteLine("  [M] Toggle Mute");
        Console.WriteLine("  [Enter] Return to main menu");

        var choice = Console.ReadKey(true).KeyChar;
        if (char.ToLower(choice) == 'v')
        {
            Console.Write("\nEnter the desired volume (0-100): ");
            var volumeInput = Console.ReadLine();
            if (float.TryParse(volumeInput, out float newVol) && newVol >= 0 && newVol <= 100)
            {
                ChangeVolume(session, newVol);
                Console.WriteLine($"Volume set to {newVol}%.");
            }
            else
            {
                Console.WriteLine("Invalid volume entered.");
            }
        }
        else if (char.ToLower(choice) == 'm')
        {
            ToggleMute(session);
            Console.WriteLine($"Mute set to {session.SimpleAudioVolume.Mute}.");
        }
        else
        {
            Console.WriteLine("No change made.");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void ShowSessionInfo(AudioSessionControl session)
    {
        float currentVolumePercent = session.SimpleAudioVolume.Volume * 100;
        bool currentlyMuted = session.SimpleAudioVolume.Mute;
        string displayName = AudioSessionHelper.GetSessionDisplayName(session);
        string filePath = AudioSessionHelper.GetProcessFilePath(session);

        Console.WriteLine($"\nSession: {displayName}");
        Console.WriteLine($"File Path: {filePath}");
        Console.WriteLine($"Current Volume: {currentVolumePercent:0.##}%");
        Console.WriteLine($"Currently Muted: {currentlyMuted}\n");
    }

    private void ChangeVolume(AudioSessionControl session, float newVolume)
        => session.SimpleAudioVolume.Volume = newVolume / 100.0f;

    private void ToggleMute(AudioSessionControl session)
        => session.SimpleAudioVolume.Mute = !session.SimpleAudioVolume.Mute;

    private float GetVolume(AudioSessionControl session)
        => session.SimpleAudioVolume.Volume;

    private bool GetMute(AudioSessionControl session)
        => session.SimpleAudioVolume.Mute;

    #endregion

    #region Group Management

    private void ManageGroups()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== GROUP MANAGEMENT ===\n");
            ListAllGroups();

            Console.WriteLine("\nOptions:");
            Console.WriteLine("  [C] Create a new group");
            Console.WriteLine("  [A] Add a session to a group");
            Console.WriteLine("  [R] Remove a session from a group");
            Console.WriteLine("  [V] Change volume of a group");
            Console.WriteLine("  [M] Toggle mute of a group");
            Console.WriteLine("  [D] Delete a group");
            Console.WriteLine("  [B] Back to main menu");

            var choice = Console.ReadKey(true).KeyChar;
            switch (char.ToLower(choice))
            {
                case 'c':
                    CreateGroup();
                    break;
                case 'a':
                    AddSessionToGroup();
                    break;
                case 'r':
                    RemoveSessionFromGroup();
                    break;
                case 'v':
                    ChangeGroupVolume();
                    break;
                case 'm':
                    ToggleGroupMute();
                    break;
                case 'd':
                    DeleteGroup();
                    break;
                case 'b':
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private void ListAllGroups()
    {
        if (_groups.Count == 0)
        {
            Console.WriteLine("No groups exist.");
            return;
        }

        Console.WriteLine("Existing Groups:");
        foreach (var kvp in _groups)
        {
            string groupName = kvp.Key;
            int sessionCount = kvp.Value.Count;
            Console.WriteLine($" - {groupName} (Sessions: {sessionCount})");
        }
    }

    private void CreateGroup()
    {
        Console.Write("\nEnter a name for the new group: ");
        string groupName = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(groupName))
        {
            Console.WriteLine("Invalid group name.");
            return;
        }

        if (_groups.ContainsKey(groupName))
        {
            Console.WriteLine("A group with that name already exists.");
        }
        else
        {
            _groups[groupName] = new List<AudioSessionControl>();
            Console.WriteLine($"Group '{groupName}' created.");
        }
    }

    private void AddSessionToGroup()
    {
        if (_groups.Count == 0)
        {
            Console.WriteLine("No groups exist. Create one first.");
            return;
        }

        var groupName = PromptGroupName();
        if (string.IsNullOrEmpty(groupName)) return;

        // Prompt for session index
        ListSessions();
        Console.Write("\nEnter session index to add to group: ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out int sessionIndex) ||
            sessionIndex < 0 || sessionIndex >= _sessions.Count)
        {
            Console.WriteLine("Invalid session index.");
            return;
        }

        var session = _sessions[sessionIndex];
        if (!_groups[groupName].Contains(session))
        {
            _groups[groupName].Add(session);
            Console.WriteLine("Session added to group.");
        }
        else
        {
            Console.WriteLine("This session is already in the group.");
        }
    }

    private void RemoveSessionFromGroup()
    {
        if (_groups.Count == 0)
        {
            Console.WriteLine("No groups exist.");
            return;
        }

        string groupName = PromptGroupName();
        if (string.IsNullOrEmpty(groupName)) return;

        var sessions = _groups[groupName];
        if (sessions.Count == 0)
        {
            Console.WriteLine($"Group '{groupName}' is empty.");
            return;
        }

        Console.WriteLine($"\nSessions in group '{groupName}':");
        for (int i = 0; i < sessions.Count; i++)
        {
            Console.WriteLine($"{i}. {AudioSessionHelper.GetSessionDisplayName(sessions[i])}");
        }

        Console.Write("\nEnter the index in this group to remove: ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out int removeIndex) ||
            removeIndex < 0 || removeIndex >= sessions.Count)
        {
            Console.WriteLine("Invalid index.");
            return;
        }

        sessions.RemoveAt(removeIndex);
        Console.WriteLine("Session removed from the group.");
    }


    private void ChangeGroupVolume()
    {
        if (_groups.Count == 0)
        {
            Console.WriteLine("No groups exist.");
            return;
        }

        var groupName = PromptGroupName();
        if (string.IsNullOrEmpty(groupName)) return;

        Console.Write("\nEnter the desired volume (0-100) for the group: ");
        var volumeInput = Console.ReadLine();
        if (!float.TryParse(volumeInput, out float newVol) || newVol < 0 || newVol > 100)
        {
            Console.WriteLine("Invalid volume.");
            return;
        }

        float normalized = newVol / 100.0f;
        foreach (var session in _groups[groupName])
        {
            session.SimpleAudioVolume.Volume = normalized;
        }
        Console.WriteLine($"All sessions in group '{groupName}' set to {newVol}% volume.");
    }

    private void ToggleGroupMute()
    {
        if (_groups.Count == 0)
        {
            Console.WriteLine("No groups exist.");
            return;
        }

        var groupName = PromptGroupName();
        if (string.IsNullOrEmpty(groupName)) return;

        var groupSessions = _groups[groupName];
        if (groupSessions.Count == 0)
        {
            Console.WriteLine($"Group '{groupName}' is empty.");
            return;
        }

        bool newState = !groupSessions[0].SimpleAudioVolume.Mute;
        foreach (var session in groupSessions)
        {
            session.SimpleAudioVolume.Mute = newState;
        }
        Console.WriteLine($"All sessions in group '{groupName}' {(newState ? "muted" : "unmuted")}.");
    }

    private void DeleteGroup()
    {
        if (_groups.Count == 0)
        {
            Console.WriteLine("No groups exist.");
            return;
        }

        Console.Write("\nEnter the name of the group to delete: ");
        string groupName = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(groupName) || !_groups.ContainsKey(groupName))
        {
            Console.WriteLine("Group not found.");
            return;
        }

        var removedSessionsCount = _groups[groupName].Count;
        _groups.Remove(groupName);

        Console.WriteLine($"Group '{groupName}' has been deleted. ({removedSessionsCount} session(s) ungrouped)");
    }

    private string PromptGroupName()
    {
        Console.Write("\nEnter the group name: ");
        string groupName = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(groupName) || !_groups.ContainsKey(groupName))
        {
            Console.WriteLine($"Group '{groupName}' not found.");
            return null;
        }
        return groupName;
    }

    #endregion
}
