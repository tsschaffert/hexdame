using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class MinimaxPlayer : AbstractComputerPlayer
    {
        private Random random;

        public MinimaxPlayer(Game.Player playerType)
            : base(playerType)
        {
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

                int score = MiniMax(newState, 1, false);

                if (score > bestValue)
                {
                    bestMove = move;
                }
            }

            return bestMove;
        }

        public int MiniMax(Gameboard state, int depth, bool max)
        {
            GameLogic gameLogic = new GameLogic(state);

            if (depth == 0 || gameLogic.IsFinished())
            {
                return Evaluate(state);
            }

            int score;

            score = max?int.MinValue:int.MaxValue;
            var possibleMoves = gameLogic.GetPossibleMoves(playerType);
            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(playerType, move);

                int value = MiniMax(newState, depth - 1, !max);
                if (max && value > score)
                {
                    score = value;
                }
                else if (!max && value < score)
                {
                    score = value;
                }
            }

            return score;
        }

        public int Evaluate(Gameboard state)
        {
            //return random.Next(-16, 17);

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
                            if (playerType == Game.Player.Red)
                            {
                                value += currentCell.ContainsKing ? 3 : 1;
                            }
                            else
                            {
                                value -= currentCell.ContainsKing ? 3 : 1;
                            }
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            if (playerType == Game.Player.Red)
                            {
                                value -= currentCell.ContainsKing ? 3 : 1;
                            }
                            else
                            {
                                value += currentCell.ContainsKing ? 3 : 1;
                            }
                        }
                    }
                }
            }

            return value;
        }
    }
}
