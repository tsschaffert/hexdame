using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class AlphaBetaParallelPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected Evaluation evaluation;

        public AlphaBetaParallelPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            evaluation = new Evaluation(playerType);
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            Move bestMove = null;
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();
            int numberOfMoves = possibleMoves.Count;
            Task<int>[] taskArray = new Task<int>[numberOfMoves];

            for (int i = 0; i < numberOfMoves; i++)
            {
                Move move = possibleMoves[i];
                Gameboard newState = (Gameboard)gameboard.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

                taskArray[i] = Task.Factory.StartNew(() => -AlphaBeta(newState, depth - 1, GameLogic.LOSS_VALUE, GameLogic.WIN_VALUE, false));
            }

            for (int i = 0; i < numberOfMoves; i++)
            {
                int score = taskArray[i].Result;
                if (taskArray[i].Result > bestValue)
                {
                    bestMove = possibleMoves[i];
                    bestValue = taskArray[i].Result;
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
