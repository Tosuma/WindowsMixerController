using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AudioMixer.DataStructure;

internal abstract record RunContext;

internal record RunAdjustVolumeContext(ProcessList Processes, int SelectedIndex) : RunContext;

internal record RunEditGroupContext(ProcessList Processes, GroupDictionary groups, int SelectedIndex) : RunContext;

internal record RunMainContext(ProcessList _processes, GroupDictionary _groups) : RunContext;