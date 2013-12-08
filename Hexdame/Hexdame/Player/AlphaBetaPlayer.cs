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
        protected Evaluation evaluation;

        public AlphaBetaPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            evaluation = new Evaluation(playerType);
        }

        public AlphaBetaPlayer(Game.Player playerType, int depth, Evaluation evaluation)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            this.evaluation = evaluation;
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            Move bestMove = null;
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

            // Don't search if only one move possible
            if(possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            foreach (Move move in possibleMoves)
            {
                Gameboard newState = (Gameboard)gameboard.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

                int score = -AlphaBeta(newState, depth - 1, GameLogic.LOSS_VALUE, GameLogic.WIN_VALUE, false);

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

            if (depth <= 0 || gameLogic.IsFinished())
            {
                return myMove ? evaluation.Evaluate(state) : -evaluation.Evaluate(state);
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
    }
}
