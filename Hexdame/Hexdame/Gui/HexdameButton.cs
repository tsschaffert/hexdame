using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hexdame
{
    class HexdameButton : CheckBox
    {
        public Position FieldPosition { set; get; }

        public HexdameButton()
        {
            this.Appearance = System.Windows.Forms.Appearance.Button;
        }
    }
}
