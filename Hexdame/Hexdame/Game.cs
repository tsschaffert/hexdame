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
        public const int PAUSE_BETWEEN_MOVES = 300;
        public const int MAX_ROUNDS = 300;

        private int currentRound;

        public enum Player { White = 0, Red = 1 }

        private bool firstGame = true;

        private Evaluator evaluator;

        public Game(GuiController guiController)
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);

            // The evaluator class can be used to get a comparison regarding the win rate for several players
            /*evaluator = new Evaluator(this,
                new AlphaBetaPlayer(Player.White, 1, new Evaluation(Player.White, 1000, 1500, 50, 50, 0)),
                new AlphaBetaPlayer(Player.White, 1, new Evaluation(Player.White, 1000, 2000, 50, 50, 0)));*/
            evaluator = new Evaluator(this);

            this.guiController = guiController;
            players = new AbstractPlayer[NUMBER_OF_PLAYERS];
            timerNextMove = new Timer(PAUSE_BETWEEN_MOVES);
            timerNextMove.Elapsed += timerNextMove_Elapsed;
            timerNextMove.AutoReset = false; 
        }

        /// <summary>
        /// Starts the game, invoked when GUI is ready.
        /// </summary>
        public void Start()
        {
            // If evaluator is active, start evaluation, otherwise, start normal game
            if (evaluator.IsActive)
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

        /// <summary>
        /// Resets the game board and players and starts the game
        /// </summary>
        public void NewGame()
        {
            gameboard = new Gameboard();
            gameLogic = new GameLogic(gameboard);

            // Loading of game state disabled
            /*if (firstGame)
            {
                firstGame = false;
                LoadState();
            }*/

            currentRound = 0;

            players[(int)Player.White] = new HumanPlayer(Player.White);
            players[(int)Player.Red] = new AlphaBetaFinalPlayer(Player.Red, 6);

            guiController.UpdateGui((Gameboard)gameboard.Clone());

            timerNextMove.Stop();
            timerNextMove.Start();
        }

        /// <summary>
        /// Takes a player move and checks if it can be performed
        /// </summary>
        public bool SendMove(Move move)
        {
            Player activePlayer = gameboard.CurrentPlayer;

            // Try to apply the move
            bool success = gameLogic.ApplyMove(move);

            if (!evaluator.IsActive)
            {
                guiController.UpdateGui((Gameboard)gameboard.Clone());
            }

            // Move allowed?
            if (success)
            {
                if (!evaluator.IsActive)
                {
                    guiController.AddMessage("Player " + activePlayer.ToString() + " made a move: " + move);
                }

                // Saving of game state disabled
                //SaveState();

                if (!gameLogic.IsFinished())
                {
                    // Ask for next move
                    timerNextMove.Start();
                }
                else
                {
                    // Game over
                    ClearState();

                    // Restart game if evaluating
                    if(evaluator.IsActive)
                    {
                        evaluator.Continue();
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Get next move from a computer player or allow human player to make a move
        /// </summary>
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
                // Computer player needs to be told to make a move
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
            if (evaluator.IsActive)
            {
                // Too many file accesses
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
            if (evaluator.IsActive)
            {
                // Too many file accesses
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
            if (evaluator.IsActive)
            {
                // Too many file accesses
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

        /// <summary>
        /// Class used to get a win rate for several computer players.
        /// </summary>
        private class Evaluator
        {
            private int[] wins;
            private List<AbstractPlayer> playerList = new List<AbstractPlayer>();
            public const int MAX_ITERATIONS = 50;
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
                game.gameboard = new Gameboard();
                game.gameLogic = new GameLogic(game.gameboard);

                game.currentRound = 0;

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
                    Player winner = GameLogic.GetNextPlayer(game.gameboard.CurrentPlayer);
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

                        if (player1Index != player2Index)
                        {
                            roundCounter++;
                            game.guiController.AddMessage(String.Format("{0}/{1}", roundCounter, playerList.Count*(playerList.Count-1)));
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
        }
    }
}
