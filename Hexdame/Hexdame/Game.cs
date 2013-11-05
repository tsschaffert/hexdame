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

        private IPlayer[] players;
        Player activePlayer;

        public const int NUMBER_OF_PLAYERS = 2;
        public enum Player { White = 0, Red = 1 }

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);
            this.guiController = guiController;
            players = new IPlayer[NUMBER_OF_PLAYERS];
            activePlayer = Player.Red;// Will be switched before first move
            NewGame();
        }

        public void NewGame()
        {
            gameboard.Reset();

            players[(int)Player.White] = new HumanPlayer();
            players[(int)Player.Red] = new HumanPlayer();

            Console.WriteLine(gameboard);
            guiController.UpdateGui(gameboard);

            NextMove();
        }

        public bool SendMove(Move move)
        {
            bool success = gameLogic.ApplyMove(activePlayer, move);

            guiController.UpdateGui((Gameboard)gameboard.Clone());

            if (success)
            {
                NextMove();
            }

            return success;
        }

        public void NextMove()
        {
            SwitchActivePlayer();

            if(players[(int)activePlayer] is IComputerPlayer)
            {
                guiController.GuiInputAllowed = false;
                IComputerPlayer computerPlayer = (IComputerPlayer)players[(int)activePlayer];
                while (!SendMove(computerPlayer.GetMove())) ;
            }
            else if (players[(int)activePlayer] is IHumanPlayer)
            {
                guiController.GuiInputAllowed = true;
            }
        }

        public void SwitchActivePlayer()
        {
            activePlayer = (Player)(((int)activePlayer + 1) % NUMBER_OF_PLAYERS);
        }
    }
}
