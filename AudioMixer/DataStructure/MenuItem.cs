using AudioMixer.Helpers;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.DataStructure;
internal class MenuItem
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

    public void DecreaseVolume(float step = 5)
        => _volume -= _volume % step == 0
            ? step
            : _volume % step;
        

    public void IncreaseVolume(float step = 5)
        => _volume += _volume % step == 0
            ? step
            : step - _volume % step;

    public void SetVolume(float volume) => _volume = volume;

    public float GetVolume() => _volume;
}
