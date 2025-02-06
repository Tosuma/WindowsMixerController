using AudioMixer.DataStructure;
using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace AudioMixer.Helpers;

internal static class AudioSessionHelper
{
    public static ProcessList GetProcessesForMenu()
    {
        var processes = new ProcessList();

        var enumerator = new MMDeviceEnumerator();
        var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        var sessionManager = device.AudioSessionManager;
        var sessions = sessionManager.Sessions;

        for (int i = 0; i < sessions.Count; i++)
        {
            AudioSessionControl session = sessions[i];

            if (session.IsSystemSoundsSession)
                continue;

            string displayName = GetSessionDisplayName(session);

            processes.Add(new(displayName, session));
        }

        return processes;
    }

    /// <summary>
    /// Attempt to get a friendly display name for this session.
    /// If not set, try to get the process name. Otherwise "Unknown/Hidden".
    /// </summary>
    public static string GetSessionDisplayName(AudioSessionControl session)
    {
        var name = session.DisplayName;

        if (!string.IsNullOrEmpty(name))
            return name;

        try
        {
            int pid = (int)session.GetProcessID;
            var proc = Process.GetProcessById(pid);
            return proc.ProcessName;
        }
        catch
        {
            return "Unknown/Hidden";
        }
    }

    /// <summary>
    /// Get the .exe file path for the underlying process (if accessible).
    /// </summary>
    public static string GetProcessFilePath(AudioSessionControl session)
    {
        try
        {
            int pid = (int)session.GetProcessID;
            if (pid == 0) return string.Empty; // System sounds, etc.

            var proc = Process.GetProcessById(pid);
            // Attempt to get the MainModule FileName
            return proc?.MainModule?.FileName ?? string.Empty;
        }
        catch
        {
            // This can fail if we don't have privileges, or the process has exited.
            return string.Empty;
        }
    }
}