using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class Game
    {
        private Gameboard gameboard;
        private GameLogic gameLogic;
        private GuiController guiController;
        public const int FIELD_SIZE = 0;

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);
            this.guiController = guiController;
            NewGame();
        }

        public void NewGame()
        {
            gameboard.Reset();

            Console.WriteLine(gameboard);
        }

        public void SendMove(Move move)
        {
            gameLogic.ApplyMove(move);
        }
    }
}
