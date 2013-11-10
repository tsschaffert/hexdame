using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexdame.Player;

namespace Hexdame
{
    class Game
    {
        private Gameboard gameboard;
        private GameLogic gameLogic;
        private GuiController guiController;

        private AbstractPlayer[] players;
        Player activePlayer;

        public const int NUMBER_OF_PLAYERS = 2;
        public enum Player { White = 0, Red = 1 }

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);
            this.guiController = guiController;
            players = new AbstractPlayer[NUMBER_OF_PLAYERS];
            activePlayer = Player.Red;// Will be switched before first move
            NewGame();
        }

        public void NewGame()
        {
            gameboard.Reset();

            players[(int)Player.White] = new HumanPlayer(Player.White);
            players[(int)Player.Red] = new MinimaxPlayer(Player.Red);

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
                if (!gameLogic.IsFinished())
                {
                    NextMove();
                }
                else
                {
                    // Spiel zu Ende
                }
            }

            return success;
        }

        public void NextMove()
        {
            SwitchActivePlayer();

            if(players[(int)activePlayer] is AbstractComputerPlayer)
            {
                guiController.GuiInputAllowed = false;
                AbstractComputerPlayer computerPlayer = (AbstractComputerPlayer)players[(int)activePlayer];
                while (!SendMove(computerPlayer.GetMove(gameboard))) ;
            }
            else if (players[(int)activePlayer] is AbstractHumanPlayer)
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
