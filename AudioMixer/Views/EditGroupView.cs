using AudioMixer.DataStructure;

namespace AudioMixer.Views;
internal class EditGroupView : IView
{
    private ProcessList _processes;
    private GroupDictionary _groups;
    private int _selectedIndex;
    private Navigator _navigator;

    public EditGroupView(ProcessList processes, GroupDictionary groups, Navigator navigator, int selectedIndex)
    {
        _processes = processes;
        _groups = groups;
        _selectedIndex = selectedIndex;
        _navigator = navigator;
    }

    public IView? HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.Escape:
                return _navigator.GroupView(_selectedIndex);
        }

        return this;
    }

    public void Render()
    {

    }
}
