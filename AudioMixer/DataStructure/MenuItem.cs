using AudioMixer.Helpers;
using NAudio.CoreAudioApi;

namespace AudioMixer.DataStructure;
public class MenuItem
{
    public string Name { get; set; }
    private float _volume
    {
        get => Session.SimpleAudioVolume.Volume * 100;
        set => Session.SimpleAudioVolume.Volume = Math.Clamp(value / 100, 0f, 100f);
    }
    public bool Muted
    {
        get => Session.SimpleAudioVolume.Mute;
        set => Session.SimpleAudioVolume.Mute = value;
    }
    public AudioSessionControl Session { get; set; }
    public string FilePath { get; set; }

    public MenuItem(string name, AudioSessionControl sessionControl)
    {
        Name = name;
        Session = sessionControl;
        FilePath = AudioSessionHelper.GetProcessFilePath(sessionControl);
    }

    public void DecreaseVolume(int step = 5)
    {
        int tempVol = (int) _volume % step; 
        _volume -= tempVol % step == 0
            ? step
            : tempVol;
    }
        

    public void IncreaseVolume(int step = 5)
    {
        int tempVol = (int) _volume % step;
        _volume += tempVol == 0
            ? step
            : step - tempVol;
    }

    public void SetVolume(float volume) => _volume = volume;

    public float GetVolume() => _volume;
}
