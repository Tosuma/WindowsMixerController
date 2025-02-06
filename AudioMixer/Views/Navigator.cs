using AudioMixer.Views;

namespace AudioMixer.Views;

internal class Navigator
{
    private ProcessList _processes;
    private GroupDictionary _groups;


    public Navigator(ProcessList processes, GroupDictionary groups)
    {
        _processes = processes;
        _groups = groups;
    }

    public void UpdateProcesses(ProcessList processes) => _processes = processes;
    public void UpdateGroups(GroupDictionary groups) => _groups = groups;


    // Main view and session handling
    public IView MainView(int selectedIndex = 0)        => new MainView(_processes, _groups, this, selectedIndex);
    public IView EditSessionView(int selectedIndex)     => new EditSessionView(_processes, _groups, this, selectedIndex);
    public IView AdjustingVolumeView(int selectedIndex) => new AdjustVolumeView(_processes, _groups, this, selectedIndex);
    
    // Group views and group handling
    public IView GroupView(int selectedIndex = 0)       => new GroupView(_processes, _groups, this, selectedIndex);
    public IView EditGroupView(int selectedIndex)       => new EditGroupView(_processes, _groups, this, selectedIndex);
}