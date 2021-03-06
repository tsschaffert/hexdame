﻿using System;
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

        private HexdameButton[] playButtons;

        public Gui()
        {
            InitializeComponent();

            playButtons = new HexdameButton[61];
            int[] buttonsPerRow = new int[] { 1, 2, 3, 4, 5, 4, 5, 4, 5, 4, 5, 4, 5, 4, 3, 2, 1 };

            int index = 0;
            for (int i = 0; i < 9; i++)
            {
                int numberOfCells = 9 - Math.Abs(9 / 2 - i);

                for (int j = 0; j < numberOfCells; j++)
                {
                    HexdameButton button = new HexdameButton();

                    int number = i + 1;
                    int character = j + 1;
                    if (i > 4)
                    {
                        character += i - 4;
                    }
                    button.FieldPosition = new Position(number, character);

                    button.Location = new Point(200 + (i>4?250:(i+1)*50) - j*50, 400 - 20 * (number + character - 2));
                    button.Width = 50;
                    button.Height = 20;

                    button.Click += button_Click;

                    playButtons[index] = button;
                    this.gamePanel.Controls.Add(button);
                    index++;
                }
            }

            buttonConfirmMove.Click += buttonConfirmMove_Click;

            controller = new GuiController(this);
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

        public void UpdateGui(Gameboard gameboard)
        {  
            foreach(HexdameButton button in playButtons)
            {
                button.Text = gameboard.GetCell(button.FieldPosition).Content.ToString();
                button.Checked = false;
                if (gameboard.GetCell(button.FieldPosition).ContainsWhite)
                {
                    button.BackColor = Color.White;
                }
                else if (gameboard.GetCell(button.FieldPosition).ContainsRed)
                {
                    button.BackColor = Color.Red;
                }
                else
                {
                    button.BackColor = Color.Transparent;
                }
            }
        }

        public void AddMessage(String message)
        {
            if (messageLog.Text == "")
            {
                messageLog.AppendText(message);
            }
            else
            {
                messageLog.AppendText("\r\n"+message);
            } 
            messageLog.ScrollToCaret();
        }

        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            controller.NewGame();
            messageLog.Clear();
        }

        public void UpdateActivePlayer(Game.Player activePlayer)
        {
            labelCurrentPlayer.Text = "Player " + activePlayer.ToString() + "'s turn.";
            labelCurrentPlayer.ForeColor = activePlayer == Game.Player.White ? Color.White : Color.Red;
        }

        private void Gui_Shown(object sender, EventArgs e)
        {
            controller.Start();
        }
    }
}
