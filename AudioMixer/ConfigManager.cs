using AudioMixer.Helpers;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AudioMixer;
[Obsolete("This class is depricated and for an older implementation of the audio mixer", true)]
internal static class ConfigManager
{
    /// <summary>
    /// Temporarily stores the data loaded from config (group -> list of SessionConfig) 
    /// until we have initialized the NAudio sessions.
    /// </summary>
    private static Dictionary<string, List<SessionConfig>> _tempLoadedGroups;

    /// <summary>
    /// A small DTO for storing each session's info in the config.
    /// </summary>
    public class SessionConfig
    {
        public string FilePath { get; set; }
        public float Volume { get; set; }  // 0..100
    }

    /// <summary>
    /// The top-level config structure: group -> list of SessionConfigs
    /// </summary>
    public class MixerConfig
    {
        public Dictionary<string, List<SessionConfig>> Groups { get; set; }
            = new Dictionary<string, List<SessionConfig>>(StringComparer.OrdinalIgnoreCase);
    }


    /// <summary>
    /// Load the config from disk, and apply it to the given AudioMixer's _groups.
    /// (You can also just store it in some temporary structure and let the AudioMixer
    /// do the mapping if you prefer.)
    /// </summary>
    public static void LoadConfiguration(string filePath, AudioMixer mixer)
    {
        if (!File.Exists(filePath)) return;

        try
        {
            string json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<MixerConfig>(json);
            if (config == null) return;

            // Clear existing groups in the mixer, or merge as you prefer
            var mixerGroups = mixer.Groups;
            mixerGroups.Clear();

            Console.WriteLine($"groups count: {config.Groups.Count}");
            foreach (var group in config.Groups)
            {
                Console.WriteLine($"name: {group.Key}: {group.Value.Count}");
            }

            // For each group in the config
            foreach (var groupEntry in config.Groups)
            {
                string groupName = groupEntry.Key;
                var sessionsInGroup = new List<AudioSessionControl>();

                // Create an empty group in the mixer
                mixerGroups[groupName] = sessionsInGroup;

                // For each session config, find active sessions that match
                // the same file path, then set volumes, etc.
                foreach (var sessCfg in groupEntry.Value)
                {
                    // We'll do a quick search of all active sessions in the mixer
                    // to see if there's a match
                    for (int i = 0; i < mixer.ActiveSessionCount; i++)
                    {
                        AudioSessionControl asc = mixer.GetSessionAtIndex(i);
                        string ascPath = AudioSessionHelper.GetProcessFilePath(asc);

                        if (string.Equals(ascPath, sessCfg.FilePath, StringComparison.OrdinalIgnoreCase))
                        {
                            // Found a match
                            sessionsInGroup.Add(asc);

                            // Optionally set the volume from config
                            asc.SimpleAudioVolume.Volume = sessCfg.Volume / 100f;
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config: {ex.Message}");
        }
    }

    /// <summary>
    /// Save the current groups from the AudioMixer to disk.
    /// </summary>
    public static void SaveConfiguration(string filePath, AudioMixer mixer)
    {
        try
        {
            var config = new MixerConfig();
            var mixerGroups = mixer.Groups;

            // For each group in the mixer
            foreach (var groupKvp in mixerGroups)
            {
                string groupName = groupKvp.Key;
                var sessionList = groupKvp.Value;

                var sessionConfigs = new List<SessionConfig>();
                foreach (var session in sessionList)
                {
                    string path = AudioSessionHelper.GetProcessFilePath(session);
                    if (string.IsNullOrEmpty(path))
                        continue; // skip system or protected sessions

                    float volPct = session.SimpleAudioVolume.Volume * 100;
                    sessionConfigs.Add(new SessionConfig
                    {
                        FilePath = path,
                        Volume = volPct
                    });
                }

                if (sessionConfigs.Count > 0)
                {
                    config.Groups.Add(groupName, sessionConfigs);
                }
            }

            // Serialize to JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config: {ex.Message}\n{ex}");
        }
    }

    /// <summary>
    /// Once _sessions is initialized, we map the loaded config 
    /// (file paths and volumes) to the actual AudioSessionControl objects 
    /// and set each session's volume accordingly.
    /// </summary>
    public static void RemapGroupsToActiveSessions(AudioMixer mixer)
    {
        if (_tempLoadedGroups == null) return;

        foreach (var kvp in _tempLoadedGroups)
        {
            string groupName = kvp.Key;
            var loadedSessions = kvp.Value;  // List<SessionConfig>

            // Clear any existing references in this group
            mixer.Groups[groupName].Clear();

            foreach (var entry in loadedSessions)
            {
                string storedPath = entry.FilePath;
                float storedVolume = entry.Volume;

                if (string.IsNullOrEmpty(storedPath))
                    continue;

                // Find active sessions that match this file path
                for (int i = 0; i < mixer.ActiveSessionCount; i++)
                {
                    var session = mixer.GetSessionAtIndex(i);
                    string activePath = AudioSessionHelper.GetProcessFilePath(session);

                    if (string.Equals(activePath, storedPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // Add to the group
                        mixer.Groups[groupName].Add(session);

                        // Apply the stored volume
                        float normalized = storedVolume / 100f;
                        session.SimpleAudioVolume.Volume = normalized;
                    }
                }
            }
        }

        // We've applied everything; clear the temp reference
        _tempLoadedGroups = null;
    }
}
