using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.DataStructure;
internal abstract record MenuActionResult(ViewState NewState);

internal record MainActionResult(ViewState NewState, int SelectedIndex, bool Running)
    : MenuActionResult(NewState);

internal record EditSessionActionResult(ViewState NewState, int EditElementIndex)
    : MenuActionResult(NewState);

internal record AdjustVolumeActionResult(ViewState NewState, bool CancelVolumeChange)
    : MenuActionResult(NewState);

internal record GroupActionResult(ViewState NewState, int SelectedIndex)
    : MenuActionResult(NewState);

internal record EditGroupActionResult(ViewState NewState)
    : MenuActionResult(NewState);