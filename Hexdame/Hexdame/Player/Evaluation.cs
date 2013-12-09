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
        protected int weightMovement = 100;

        protected int maxRandom = 999;

        protected Random random;
        protected Game.Player playerType; 

        public Evaluation(Game.Player playerType)
        {
            random = new Random();
            this.playerType = playerType;
        }

        public Evaluation(Game.Player playerType, int weightMan, int weightKing, int weightMovement, int maxRandom)
            : this(playerType)
        {
            this.weightMan = weightMan;
            this.weightKing = weightKing;
            this.weightMovement = weightMovement;
            this.maxRandom = maxRandom;
        }

        public int Evaluate(Gameboard state)
        {
            int value = 0;

            // Evaluation of piece count
            int valuePieces = EvaluatePieces(state);

            if (valuePieces == GameLogic.LOSS_VALUE || valuePieces == GameLogic.WIN_VALUE)
            {
                return valuePieces;
            }

            int valueMovement = EvaluateMovement(state);

            value += valuePieces;
            //value += valueMovement;

            //value += random.Next(-maxRandom, maxRandom+1);

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
            GameLogic gameLogic = new GameLogic(state);

            var possibleMoves = gameLogic.GetPossibleWithoutLookingAtMaxCaptures();

            int piecesAbleToMove = 1;

            Position lastStartPosition = possibleMoves[0].GetStartingPosition();

            foreach (Move move in possibleMoves)
            {
                if (move.GetStartingPosition() != lastStartPosition)
                {
                    piecesAbleToMove++;
                    lastStartPosition = move.GetStartingPosition();
                }
            }

            return piecesAbleToMove * weightMovement;
        }
    }
}
