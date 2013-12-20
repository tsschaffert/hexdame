using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    public class GuiController
    {
        public delegate void InvokeDelegateUpdateGui(Gameboard gameboard);
        public delegate void InvokeDelegateAddMessage(String message);
        public delegate void InvokeDelegateUpdateActivePlayer(Game.Player activePlayer);

        private Game game;
        public Gui gui;
        private bool guiInputAllowed;

        public bool GuiInputAllowed { set { guiInputAllowed = value; } get { return guiInputAllowed; } }

        private List<Position> currentPositions;

        public GuiController(Gui gui)
        {
            this.gui = gui;
            game = new Game(this);

            currentPositions = new List<Position>();
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
            if (gui.Created)
            {
                gui.BeginInvoke(new InvokeDelegateUpdateGui(gui.UpdateGui), new object[] { gameboard });
            }
        }

        public void AddMessage(String message)
        {
            if (gui.Created)
            {
                gui.BeginInvoke(new InvokeDelegateAddMessage(gui.AddMessage), new object[] { message });
            }
        }

        public void NewGame()
        {
            game.NewGame();
        }

        public void UpdateActivePlayer(Game.Player activePlayer)
        {
            if (gui.Created)
            {
                gui.BeginInvoke(new InvokeDelegateUpdateActivePlayer(gui.UpdateActivePlayer), new object[] { activePlayer });
            }
        }

        public void Start()
        {
            game.Start();
        }
    }
}
