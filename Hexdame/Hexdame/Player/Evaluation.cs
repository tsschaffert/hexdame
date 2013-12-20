using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    class Evaluation
    {
        protected int weightMan = 1000;
        protected int weightKing = 1500;
        protected int weightMovementMan = 0;
        protected int weightMovementKing = 0;

        protected int maxRandom = 0;

        protected Random random;
        protected Game.Player playerType; 

        public Game.Player PlayerType
        {
            set { playerType = value; }
            get { return playerType; }
        }

        public Evaluation(Game.Player playerType)
        {
            random = new Random();
            this.playerType = playerType;
        }

        public Evaluation(Game.Player playerType, int weightMan, int weightKing, int weightMovementMan, int weightMovementKing, int maxRandom)
            : this(playerType)
        {
            this.weightMan = weightMan;
            this.weightKing = weightKing;
            this.weightMovementMan = weightMovementMan;
            this.weightMovementKing = weightMovementKing;
            this.maxRandom = maxRandom;
        }

        /// <summary>
        /// Returns the evaluation for the given game state. Evaluation if always from the perspective
        /// of the computer player using it. So for NegaMax, it has to be negated in opponents turn.
        /// </summary>
        public int Evaluate(Gameboard state)
        {
            int value = 0;

            // Evaluation of piece count
            int valuePieces = EvaluatePieces(state);

            // If win or loss, no further evaluation needed
            if (valuePieces == GameLogic.LOSS_VALUE || valuePieces == GameLogic.WIN_VALUE)
            {
                return valuePieces;
            }

            int valueMovement = EvaluateMovement(state);

            // If win or loss, no further evaluation needed
            if (valueMovement == GameLogic.LOSS_VALUE || valueMovement == GameLogic.WIN_VALUE)
            {
                return valueMovement;
            }

            value += valuePieces;
            value += valueMovement;

            if(maxRandom != 0)
            {
                value += random.Next(-maxRandom, maxRandom + 1);
            }

            return value;
        }

        protected int EvaluatePieces(Gameboard state)
        {
            // Number of pieces on the board
            int whiteMen = 0;
            int whiteKings = 0;
            int redMen = 0;
            int redKings = 0;
            for (int i = 1; i <= Gameboard.FIELD_SIZE; i++)
            {
                for (int j = 1; j <= Gameboard.FIELD_SIZE; j++)
                {
                    Position currentPosition = new Position(i, j);

                    if (state.ValidCell(currentPosition))
                    {
                        Cell currentCell = state.GetCell(currentPosition);
                        // Player man on current position?
                        if (currentCell.ContainsRed)
                        {
                            if (currentCell.ContainsKing)
                            {
                                redKings++;
                            }
                            else
                            {
                                redMen++;
                            }
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            if (currentCell.ContainsKing)
                            {
                                whiteKings++;
                            }
                            else
                            {
                                whiteMen++;
                            }
                        }
                    }
                }
            }
            // Calculate value for player White
            int valuePieces = whiteKings * weightKing + whiteMen * weightMan - redKings * weightKing - redMen * weightMan;
            // If red, negate
            if (playerType == Game.Player.Red)
            {
                valuePieces = -valuePieces;
            }

            // Check for win
            if ((whiteKings + whiteMen) == 0)
            {
                return playerType == Game.Player.White ? GameLogic.LOSS_VALUE : GameLogic.WIN_VALUE;
            }
            else if ((redKings + redMen) == 0)
            {
                return playerType == Game.Player.Red ? GameLogic.LOSS_VALUE : GameLogic.WIN_VALUE;
            }

            return valuePieces;
        }

        protected int EvaluateMovement(Gameboard state)
        {
            if(weightMovementMan == 0 && weightMovementKing == 0)
            {
                return 0;
            }

            // Get movement value for current player
            int valuePlayer = EvaluateMovementForPlayer(state, playerType);

            // Check if we are out of moves
            if(valuePlayer == GameLogic.LOSS_VALUE)
            {
                return valuePlayer;
            }

            int valueOpponent = EvaluateMovementForPlayer(state, GameLogic.GetNextPlayer(playerType));

            // Check if opponent is out of moves
            if(valueOpponent == GameLogic.WIN_VALUE)
            {
                return valueOpponent;
            }

            return valuePlayer - valueOpponent;
        }

        protected int EvaluateMovementForPlayer(Gameboard state, Game.Player player)
        {
            Game.Player oldPlayer = state.CurrentPlayer;
            state.CurrentPlayer = player;

            GameLogic gameLogic = new GameLogic(state);

            var possibleMoves = gameLogic.GetPossibleWithoutLookingAtMaxCaptures();

            if (possibleMoves.Count == 0)
            {
                // No more moves for me
                if(player == playerType)
                {
                    return GameLogic.LOSS_VALUE;
                }
                // No more moves for the opponent
                else
                {
                    return GameLogic.WIN_VALUE;
                }
            }

            int menAbleToMove = 0;
            int kingsAbleToMove = 0;

            Position lastStartPosition = possibleMoves[0].GetStartingPosition();

            if (state.GetCell(lastStartPosition).ContainsKing)
            {
                kingsAbleToMove++;
            }
            else
            {
                menAbleToMove++;
            }

            foreach (Move move in possibleMoves)
            {
                if (move.GetStartingPosition() != lastStartPosition)
                {
                    lastStartPosition = move.GetStartingPosition();

                    if (state.GetCell(lastStartPosition).ContainsKing)
                    {
                        kingsAbleToMove++;
                    }
                    else
                    {
                        menAbleToMove++;
                    }
                }
            }

            state.CurrentPlayer = oldPlayer;

            return menAbleToMove * weightMovementMan + kingsAbleToMove * weightMovementKing;
        }
    }
}
