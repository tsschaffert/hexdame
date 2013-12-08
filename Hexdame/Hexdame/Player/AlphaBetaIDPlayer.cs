using Hexdame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    class AlphaBetaIDPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected LimitedSizeDictionary<Int64, Transposition> transpositionTable;
        protected Evaluation evaluation;

        public AlphaBetaIDPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            transpositionTable = new LimitedSizeDictionary<Int64, Transposition>();
            evaluation = new Evaluation(playerType);
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            Move bestMove = null;
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

            for (int currentDepth = 1; currentDepth <= depth; currentDepth++)
            {
                Move currentBestMove = null;
                int currentBestValue = int.MinValue;

                foreach (Move move in possibleMoves)
                {
                    Gameboard newState = (Gameboard)gameboard.Clone();
                    GameLogic newLogic = new GameLogic(newState);
                    newLogic.ApplyMove(move);

                    int score = -AlphaBeta(newState, currentDepth - 1, GameLogic.LOSS_VALUE, GameLogic.WIN_VALUE, false);

                    if (score > currentBestValue)
                    {
                        currentBestMove = move;
                        currentBestValue = score;
                    }
                }

                if (currentBestValue > bestValue)
                {
                    bestValue = currentBestValue;
                    bestMove = currentBestMove;
                }
            }

            return bestMove;
        }

        public int AlphaBeta(Gameboard state, int depth, int alpha, int beta, bool myMove)
        {
            Int64 zHash = state.GetZobristHash();
            if (transpositionTable.ContainsKey(zHash))
            {
                Transposition transposition = transpositionTable[zHash];

                if (transposition.Depth >= depth)
                {
                    // TODO possible to just return exact value?
                    if (transposition.Lowerbound == transposition.Upperbound)
                    {
                        return transposition.Lowerbound;
                    }

                    if (transposition.Lowerbound >= beta)
                    {
                        return transposition.Lowerbound;
                    }
                    if (transposition.Upperbound <= alpha)
                    {
                        return transposition.Upperbound;
                    }
                    alpha = Math.Max(alpha, transposition.Lowerbound);
                    beta = Math.Min(beta, transposition.Upperbound);
                }
            }

            GameLogic gameLogic = new GameLogic(state);
            int score;

            if (depth <= 0 || gameLogic.IsFinished())
            {
                score = myMove ? evaluation.Evaluate(state) : -evaluation.Evaluate(state);
            }
            else
            {
                score = int.MinValue;
                var possibleMoves = gameLogic.GetPossibleMoves();

                OrderMoves(possibleMoves, state);

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
            }

            if (score <= alpha)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(alpha, score, depth, null);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                }
            }
            else if (score > alpha && score < beta)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(score, score, depth, null);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Lowerbound = score;
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                }
            }
            else if (score >= beta)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(score, beta, depth, null);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Lowerbound = score;
                    transposition.Depth = depth;
                }
            }

            return score;
        }

        public void OrderMoves(List<Move> moves, Gameboard state)
        {
            // Assign values to moves, if possible
            for (int i = 0; i < moves.Count; i++)
            {
                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(moves[i]);

                Int64 zHash = newState.GetZobristHash();
                if (transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = transpositionTable[zHash];
                    if (transposition.Lowerbound == transposition.Upperbound)
                    {
                        moves[i].Value = transposition.Lowerbound;
                    }
                }
            }
            moves.Sort(MoveComparison);
        }

        public static int MoveComparison(Move m1, Move m2)
        {
            return m1.Value - m2.Value;
        }
    }
}
