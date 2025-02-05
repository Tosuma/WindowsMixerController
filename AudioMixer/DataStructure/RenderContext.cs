using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.DataStructure;
internal abstract record RenderContext;

internal record RenderAdjustVolumeContext(ProcessList Processes, int SelectedIndex) : RenderContext;

internal record RenderEditGroupContext(ProcessList Processes, GroupDictionary Groups, int SelectedIndex) : RenderContext;
