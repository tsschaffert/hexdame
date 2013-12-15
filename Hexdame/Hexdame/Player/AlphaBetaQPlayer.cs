using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    class AlphaBetaQPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected Evaluation evaluation;

        // DEBUG
        private int iterationCounter;
        private int n;

        public AlphaBetaQPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            evaluation = new Evaluation(playerType);
        }

        public AlphaBetaQPlayer(Game.Player playerType, int depth, Evaluation evaluation)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            this.evaluation = evaluation;
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            List<Move> bestMove = new List<Move>();
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

            // Don't search if only one move possible
            if(possibleMoves.Count == 1)
            {
                // DEBUG
                n++;
                Console.WriteLine("Average Nodes: {0}, n={1}", iterationCounter / n, n);

                return possibleMoves[0];
            }

            int alpha = GameLogic.LOSS_VALUE;
            int beta = GameLogic.WIN_VALUE;

            foreach (Move move in possibleMoves)
            {
                // DEBUG
                iterationCounter++;

                Gameboard newState = (Gameboard)gameboard.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

                int score = -AlphaBeta(newState, depth - 1, -beta, -alpha, false);

                if (score > alpha)
                {
                    alpha = score;
                }

                if (score > bestValue)
                {
                    bestMove.Clear();
                    bestMove.Add(move);
                    bestValue = score;
                }
                else if(score == bestValue)
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

        public int AlphaBeta(Gameboard state, int depth, int alpha, int beta, bool myMove)
        {
            // DEBUG
            iterationCounter++;

            GameLogic gameLogic = new GameLogic(state);

            if (depth <= 0 || gameLogic.IsFinished())
            {
                return -QuiescenceSearch(state, -beta, -alpha, !myMove);
            }

            int score = int.MinValue;
            var possibleMoves = gameLogic.GetPossibleMoves();

            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

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

        public int QuiescenceSearch(Gameboard state, int alpha, int beta, bool myMove)
        {
            // DEBUG
            iterationCounter++;

            int score = myMove ? evaluation.Evaluate(state) : -evaluation.Evaluate(state);

            if(score >= beta)
            {
                return score;
            }

            if(score > alpha)
            {
                alpha = score;
            }

            GameLogic gameLogic = new GameLogic(state);
            var possibleMoves = gameLogic.GetPossibleCaptureMoves();

            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

                int value = -QuiescenceSearch(newState, -beta, -alpha, !myMove);
                if (value > score)
                {
                    score = value;
                    if (score >= beta)
                    {
                        break;
                    }
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            }

            return score;
        }
    }
}
