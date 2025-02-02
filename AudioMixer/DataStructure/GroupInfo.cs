using System.Text.Json.Serialization;

namespace AudioMixer.DataStructure;

public class GroupInfo
{
    private float _volume;
    [JsonPropertyName("volume")]
    public float Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0f, 100f);
    }

    [JsonPropertyName("sessions")]
    public List<string> SessionNames { get; set; } = new();

    [JsonPropertyName("muted")]
    public bool Muted { get; set; } = false;
}

