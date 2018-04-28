using System;
using System.Windows.Forms;

namespace Rhinoceros
{
    public class Container: Panel
    {
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == 0x84)
            {
                m.Result = new IntPtr(-1); //register mouse down event as transparent
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
