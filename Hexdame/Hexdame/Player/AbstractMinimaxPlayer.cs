using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    abstract class AbstractMinimaxPlayer : AbstractComputerPlayer
    {
        protected Random random;
        protected readonly int depth;

        // DEBUG
        private int iterationCounter;
        private int n;

        public AbstractMinimaxPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            random = new Random();
            this.depth = depth;
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            List<Move> bestMove = new List<Move>();
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

            foreach (Move move in possibleMoves)
            {
                // DEBUG
                iterationCounter++;

                Gameboard newState = (Gameboard)gameboard.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

                int score = MiniMax(newState, depth - 1, false);

                if (score > bestValue)
                {
                    bestMove.Clear();
                    bestMove.Add(move);
                    bestValue = score;
                }
                else if (score == bestValue)
                {
                    bestMove.Add(move);
                }
            }

            // DEBUG
            n++;
            Console.WriteLine("Average Nodes: {0}, n={1}", iterationCounter / n, n);

            // Return one of the best moves
            return bestMove[random.Next(bestMove.Count)];
        }

        public int MiniMax(Gameboard state, int depth, bool max)
        {
            // DEBUG
            iterationCounter++;

            GameLogic gameLogic = new GameLogic(state);

            if (depth <= 0 || gameLogic.IsFinished())
            {
                return Evaluate(state);
            }

            int score;

            score = max?int.MinValue:int.MaxValue;
            var possibleMoves = gameLogic.GetPossibleMoves();
            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

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

        public abstract int Evaluate(Gameboard state);
    }
}
