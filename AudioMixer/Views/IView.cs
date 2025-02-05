using AudioMixer.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.Views;
internal interface IView
{
    //public void Run();
    public IView? HandleInput(ConsoleKey key);
    public void Render();
}
