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
        private GuiController controller;

        public Gui()
        {
            InitializeComponent();

            controller = new GuiController(this);

            buttonConfirmMove.Click += buttonConfirmMove_Click;

            hexdameButton1.Click += button_Click;
            hexdameButton2.Click += button_Click;

            hexdameButton1.FieldPosition = new Position(4, 'd');
            hexdameButton2.FieldPosition = new Position(4, 'e');
        }

        void buttonConfirmMove_Click(object sender, EventArgs e)
        {
            controller.ConfirmMove();
        }

        void button_Click(object sender, EventArgs e)
        {
            if (sender is HexdameButton)
            {
                HexdameButton hexButton = (HexdameButton)sender;

                controller.SendPosition(hexButton.FieldPosition);
            }
        }
    }
}
