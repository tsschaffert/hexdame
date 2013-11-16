using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    class AlphaBetaPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;

        public AlphaBetaPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            Move bestMove = null;
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves(playerType);

            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)gameboard.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(playerType, move);

                int score = -AlphaBeta(newState, depth - 1, int.MinValue, int.MaxValue, false);

                if (score > bestValue)
                {
                    bestMove = move;
                    bestValue = score;
                }
            }

            return bestMove;
        }

        public int AlphaBeta(Gameboard state, int depth, int alpha, int beta, bool myMove)
        {
            GameLogic gameLogic = new GameLogic(state);
            Game.Player activePlayer = myMove ? playerType : (Game.Player)(1 - (int)playerType); // TODO

            if (depth <= 0 || gameLogic.IsFinished())
            {
                return Evaluate(state, activePlayer);
            }

            int score = int.MinValue;
            var possibleMoves = gameLogic.GetPossibleMoves(activePlayer);

            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(activePlayer, move);

                int value = -AlphaBeta(newState, depth - 1, -beta, -alpha, !myMove);
                if (value > score)
                {
                    score = value;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
                if (score >= beta)
                {
                    break;
                }
            }

            return score;
        }

        public int Evaluate(Gameboard state, Game.Player activePlayer)
        {
            int value = 0;

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
                            if (activePlayer == Game.Player.Red)
                            {
                                value += currentCell.ContainsKing ? 30 : 10;
                            }
                            else
                            {
                                value -= currentCell.ContainsKing ? 30 : 10;
                            }
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            if (activePlayer == Game.Player.Red)
                            {
                                value -= currentCell.ContainsKing ? 30 : 10;
                            }
                            else
                            {
                                value += currentCell.ContainsKing ? 30 : 10;
                            }
                        }
                    }
                }
            }

            value += random.Next(-9, 10);

            return value;
        }
    }
}
