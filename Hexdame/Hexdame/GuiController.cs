using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class GuiController
    {
        private Game game;
        private Gui gui;
        private bool guiInputAllowed;

        public bool GuiInputAllowed { set { guiInputAllowed = value; } get { return guiInputAllowed; } }

        private List<Position> currentPositions;

        public GuiController(Gui gui)
        {
            this.gui = gui;
            game = new Game(this);

            currentPositions = new List<Position>();

            //game.SendMove(new Move(new Position(3,3), new Position(3,4)));
        }

        public void SendPosition(Position position)
        {
            if (!GuiInputAllowed)
            {
                return;
            }

            if (currentPositions.Count != 0 && position == currentPositions[currentPositions.Count - 1])
            {
                currentPositions.RemoveAt(currentPositions.Count - 1);
            }
            else
            {
                currentPositions.Add(position);
            }
        }

        public void ConfirmMove()
        {
            game.SendMove(new Move(currentPositions.ToArray<Position>()));
            currentPositions.Clear();
        }

        public void UpdateGui(Gameboard gameboard)
        {
            gui.UpdateGui(gameboard);
        }
    }
}
