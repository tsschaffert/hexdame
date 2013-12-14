using Hexdame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    class AlphaBetaPVSPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected LimitedSizeDictionary<Int64, Transposition> transpositionTable;
        protected Evaluation evaluation;

        // DEBUG
        private int iterationCounter;
        private int n;

        public AlphaBetaPVSPlayer(Game.Player playerType, int depth)
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
            List<Move> bestMove = new List<Move>();
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

            // Don't search if only one move possible
            if (possibleMoves.Count == 1)
            {
                // DEBUG
                n++;
                Console.WriteLine("Average Nodes: {0}, n={1}", iterationCounter / n, n);

                return possibleMoves[0];
            }

            for (int currentDepth = 1; currentDepth <= depth; currentDepth++)
            {
                List<Move> currentBestMove = new List<Move>();
                int currentBestValue = int.MinValue;

                //if(currentDepth > 1)
                {
                    OrderMoves(possibleMoves, gameboard);
                }

                foreach (Move move in possibleMoves)
                {
                    // DEBUG
                    iterationCounter++;

                    Gameboard newState = (Gameboard)gameboard.Clone();
                    GameLogic newLogic = new GameLogic(newState);
                    newLogic.ApplyMove(move);

                    int score = -AlphaBeta(newState, currentDepth - 1, GameLogic.LOSS_VALUE, GameLogic.WIN_VALUE, false);

                    if (score > currentBestValue)
                    {
                        currentBestMove.Clear();
                        currentBestMove.Add(move);
                        currentBestValue = score;
                    }
                    else if(score == currentBestValue)
                    {
                        currentBestMove.Add(move);
                    }
                }

                if (currentBestValue > bestValue)
                {
                    bestValue = currentBestValue;
                    bestMove.Clear();
                    bestMove.AddRange(currentBestMove);
                }
                else if(currentBestValue == bestValue)
                {
                    bestMove.AddRange(currentBestMove);
                }
            }

            // DEBUG
            n++;
            Console.WriteLine("Average Nodes: {0}, n={1}", iterationCounter / n, n);

            // DEBUG
            //transpositionTable.Clear();

            // Return one of the best moves
            return bestMove[random.Next(bestMove.Count)];
        }

        public int AlphaBeta(Gameboard state, int depth, int alpha, int beta, bool myMove)
        {
            // DEBUG
            iterationCounter++;

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

                if (depth > 1)
                {
                    OrderMoves(possibleMoves, state);
                }

                {
                    Move pvsMove = possibleMoves[0];

                    Gameboard newState = (Gameboard)state.Clone();
                    GameLogic newLogic = new GameLogic(newState);
                    newLogic.ApplyMove(pvsMove);

                    score = -AlphaBeta(newState, depth - 1, -beta, -alpha, !myMove);
                }

                if (score < beta)
                {
                    for (int i = 1; i < possibleMoves.Count; i++)
                    {
                        Move move = possibleMoves[i];
                        int lowerbound = Math.Max(alpha, score);
                        int upperbound = lowerbound + 1;

                        Gameboard newState = (Gameboard)state.Clone();
                        GameLogic newLogic = new GameLogic(newState);
                        newLogic.ApplyMove(move);

                        int value = -AlphaBeta(newState, depth - 1, -upperbound, -lowerbound, !myMove);

                        // Fail high
                        if(value >= upperbound && value < beta)
                        {
                            value = -AlphaBeta(newState, depth - 1, -beta, -value, !myMove);
                        }

                        if (value > score)
                        {
                            score = value;
                        }
                        /*if (score > alpha)
                        {
                            alpha = score;
                        }*/
                        if (score >= beta)
                        {
                            break;
                        }
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
            // TODO maybe negate?
            return m1.Value.CompareTo(m2.Value);
        }
    }
}
