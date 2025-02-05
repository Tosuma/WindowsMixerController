using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.DataStructure;

internal abstract record HandleContext;

internal record HandleAdjustVolumeContext(ConsoleKey Key, MenuItem Process) : HandleContext;

internal record HandleEditGroupContext(ConsoleKey Key) : HandleContext;
internal record HandleMainContext(ConsoleKey key, int selectedIndex, int numOfProcesses) : HandleContext;