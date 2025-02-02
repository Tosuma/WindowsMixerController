using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AudioMixer.DataStructure;
public class GroupConfig
{
    [JsonPropertyName("groups")]
    public Dictionary<string, GroupInfo> Groups { get; set; } = new();
}

