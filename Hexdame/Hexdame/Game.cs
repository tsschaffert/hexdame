using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexdame.Player;
using System.Timers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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
        public const int PAUSE_BETWEEN_MOVES = 1;
        public const int MAX_ROUNDS = 300;

        private int currentRound;

        public enum Player { White = 0, Red = 1 }

        private Stack<Gameboard> gameHistory;

        private bool firstGame = true;

        private Evaluator evaluator;

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);
            gameHistory = new Stack<Gameboard>();

            /*evaluator = new Evaluator(this,
                new AlphaBetaPlayer(Player.White, 2),
                new AlphaBetaIDPlayer(Player.White, 2)
                );*/
            evaluator = new Evaluator(this);

            this.guiController = guiController;
            players = new AbstractPlayer[NUMBER_OF_PLAYERS];
            timerNextMove = new Timer(PAUSE_BETWEEN_MOVES);
            timerNextMove.Elapsed += timerNextMove_Elapsed;
            timerNextMove.AutoReset = false;
            if(evaluator.IsActive)
            {
                evaluator.Start();
            }
            else
            {
                NewGame();
            }
        }

        void timerNextMove_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerNextMove.Stop();
            NextMove();
        }

        public void NewGame()
        {
            gameboard.Reset();

            if (firstGame)
            {
                firstGame = false;
                LoadState();
            }

            currentRound = 0;

            gameHistory.Clear();
            gameHistory.Push((Gameboard)gameboard.Clone());

            players[(int)Player.White] = new AlphaBetaIDPlayer(Player.White, 6);
            players[(int)Player.Red] = new RandomPlayer(Player.Red);

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

            if (!evaluator.IsActive)
            {
                guiController.UpdateGui((Gameboard)gameboard.Clone());
            }

            if (success)
            {
                if (!evaluator.IsActive)
                {
                    guiController.AddMessage("Player " + activePlayer.ToString() + " made a move: " + move);
                }

                // Save current state
                gameHistory.Push((Gameboard)gameboard.Clone());
                SaveState();

                if (!gameLogic.IsFinished())
                {
                    timerNextMove.Start();
                }
                else
                {
                    // Spiel zu Ende
                    ClearState();

                    if(evaluator.IsActive)
                    {
                        evaluator.Continue();
                    }
                }
            }

            return success;
        }

        public void NextMove()
        {
            Game.Player activePlayer = gameboard.CurrentPlayer;

            // If evaluating, abort match after MAX_ROUNDS
            if (evaluator.IsActive)
            {
                currentRound++;
                if (currentRound > MAX_ROUNDS)
                {
                    // Abort game
                    evaluator.Continue();
                    return;
                }
            }

            if (!evaluator.IsActive)
            {
                guiController.UpdateActivePlayer(activePlayer);
            }

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

        private void LoadState()
        {
            // Too many file accesses
            if (evaluator.IsActive)
            {
                return;
            }

            try
            {
                using (Stream stream = new FileStream("last_state.xml", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    gameboard = (Gameboard)bf.Deserialize(stream);
                    gameLogic = new GameLogic(gameboard);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void SaveState()
        {
            // Too many file accesses
            if (evaluator.IsActive)
            {
                return;
            }

            try
            {
                using (Stream stream = new FileStream("last_state.xml", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, gameboard);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void ClearState()
        {
            // Too many file accesses
            if (evaluator.IsActive)
            {
                return;
            }

            try
            {
                File.Delete("last_state.xml");
            }
            catch(Exception e)
            {

            }
        }

        private class Evaluator
        {
            private int[] wins;
            private List<AbstractPlayer> playerList = new List<AbstractPlayer>();
            public const int MAX_ITERATIONS = 4;
            private bool active;
            private Game game;

            private int player1Index;
            private int player2Index;
            private int iteration;

            private int roundCounter;

            public bool IsActive
            {
                get { return active; }
            }

            public Evaluator(Game game, params AbstractPlayer[] players)
            {
                this.game = game;
                if (players.Length < 2)
                {
                    active = false;
                }
                else
                {
                    wins = new int[players.Length];
                    foreach (AbstractPlayer p in players)
                    {
                        playerList.Add(p);
                    }
                    active = true;
                }
            }

            public void Start()
            {
                player1Index = 0;
                player2Index = 1;
                iteration = 0;

                roundCounter = 0;

                if(playerList.Count >= 2)
                {
                    active = true;
                    wins = new int[playerList.Count];

                    RestartGame();
                }
            }

            private void RestartGame()
            {
                game.gameboard.Reset();

                game.currentRound = 0;

                game.gameHistory.Clear();
                game.gameHistory.Push((Gameboard)game.gameboard.Clone());

                // Set player colours
                playerList[player1Index].ChangePlayerType(Player.White);
                playerList[player2Index].ChangePlayerType(Player.Red);

                game.players[(int)Player.White] = playerList[player1Index];
                game.players[(int)Player.Red] = playerList[player2Index];

                game.guiController.UpdateGui((Gameboard)game.gameboard.Clone());

                game.timerNextMove.Stop();
                game.timerNextMove.Start();
            }

            public void Continue()
            {
                if(active == false)
                {
                    return;
                }

                if(game.gameLogic.IsFinished())
                {
                    // Get winner
                    Player winner = game.gameLogic.GetNextPlayer(game.gameboard.CurrentPlayer);
                    if(winner == Player.White)
                    {
                        wins[player1Index]++;
                    }
                    else
                    {
                        wins[player2Index]++;
                    }
                }

                do
                {
                    iteration++;
                    if (iteration >= MAX_ITERATIONS)
                    {
                        iteration = 0;

                        // DEBUG
                        if (player1Index != player2Index)
                        {
                            roundCounter++;
                            game.guiController.AddMessage(String.Format("{0}/{1}", roundCounter, Fac(playerList.Count)));
                        }

                        player2Index = (player2Index + 1) % playerList.Count;
                        if (player2Index == 0)
                        {
                            player1Index++;
                            if (player1Index >= playerList.Count)
                            {
                                active = false;

                                Console.WriteLine("Player evaluation: ");
                                for (int i = 0; i < playerList.Count; i++)
                                {
                                    Console.WriteLine("\tPlayer {0}: {1}", i, wins[i]);
                                }

                                game.guiController.AddMessage("Evaluation finished.");

                                break;
                            }
                        }
                    }
                } while (player1Index == player2Index);

                if (active == false)
                {
                    return;
                }

                RestartGame();
            }

            private static int Fac(int n)
            {
                int f = 1;
                for(int i=2;i<=n;i++)
                {
                    f *= i;
                }
                return f;
            }
        }
    }
}
