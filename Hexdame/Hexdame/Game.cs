using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexdame.Player;
using System.Timers;

namespace Hexdame
{
    public class Game
    {
        private Gameboard gameboard;
        private GameLogic gameLogic;
        private GuiController guiController;

        private Timer timerNextMove;

        private AbstractPlayer[] players;

        public const int NUMBER_OF_PLAYERS = 2;
        public enum Player { White = 0, Red = 1 }

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);
            this.guiController = guiController;
            players = new AbstractPlayer[NUMBER_OF_PLAYERS];
            timerNextMove = new Timer(500);
            timerNextMove.Elapsed += timerNextMove_Elapsed;
            NewGame();
        }

        void timerNextMove_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerNextMove.Stop();
            NextMove();
        }

        public void NewGame()
        {
            gameboard.Reset();

            players[(int)Player.White] = new AlphaBetaTTPlayer(Player.White, 4);
            players[(int)Player.Red] = new AlphaBetaPlayer(Player.Red, 4);

            //Console.WriteLine(gameboard);
            guiController.UpdateGui(gameboard);

            timerNextMove.Stop();
            timerNextMove.Start();
        }

        public bool SendMove(Move move)
        {
            Player activePlayer = gameboard.CurrentPlayer;
            bool success = gameLogic.ApplyMove(move);

            guiController.UpdateGui((Gameboard)gameboard.Clone());

            if (success)
            {
                guiController.AddMessage("Player " + activePlayer.ToString() + " made a move: " + move);

                if (!gameLogic.IsFinished())
                {
                    timerNextMove.Start();
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
            Game.Player activePlayer = gameboard.CurrentPlayer;
            guiController.UpdateActivePlayer(activePlayer);

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
    }
}
