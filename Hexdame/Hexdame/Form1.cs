using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hexdame
{
    public partial class Gui : Form
    {
        private Game game;

        public Gui()
        {
            InitializeComponent();

            game = new Game();
        }
    }
}
