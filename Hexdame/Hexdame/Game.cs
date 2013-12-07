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

        private Stack<Gameboard> gameHistory;

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);
            gameHistory = new Stack<Gameboard>();
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

            gameHistory.Clear();
            gameHistory.Push((Gameboard)gameboard.Clone());

            players[(int)Player.White] = new AlphaBetaPlayer(Player.White, 4);
            players[(int)Player.Red] = new AlphaBetaIDPlayer(Player.Red, 4);

            guiController.UpdateGui((Gameboard)gameboard.Clone());

            timerNextMove.Stop();
            timerNextMove.Start();
        }

        public void UndoLastMove()
        {
            if(gameHistory.Count > 0)
            {
                gameboard = gameHistory.Pop();
                gameLogic = new GameLogic(gameboard);

                guiController.UpdateGui((Gameboard)gameboard.Clone());

                timerNextMove.Stop();
                timerNextMove.Start();
            }
        }

        public bool SendMove(Move move)
        {
            Player activePlayer = gameboard.CurrentPlayer;
            bool success = gameLogic.ApplyMove(move);

            guiController.UpdateGui((Gameboard)gameboard.Clone());

            if (success)
            {
                guiController.AddMessage("Player " + activePlayer.ToString() + " made a move: " + move);
                gameHistory.Push((Gameboard)gameboard.Clone());

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
