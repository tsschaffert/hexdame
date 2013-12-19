using Hexdame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexdame.Player
{
    class AlphaBetaFinalPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected LimitedSizeDictionary<Int64, Transposition> transpositionTable;
        protected Evaluation evaluation;

        // DEBUG
        private int iterationCounter;
        private int n;

        public AlphaBetaFinalPlayer(Game.Player playerType, int depth)
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

            int startDepth = (depth % 2 == 0) ? 2 : 1;

            for (int currentDepth = startDepth; currentDepth <= depth; currentDepth += 2)
            {
                bestValue = int.MinValue;
                bestMove.Clear();

                OrderMoves(possibleMoves, gameboard);

                int alpha = GameLogic.LOSS_VALUE;
                int beta = GameLogic.WIN_VALUE;

                foreach (Move move in possibleMoves)
                {
                    // DEBUG
                    iterationCounter++;

                    Gameboard newState = (Gameboard)gameboard.Clone();
                    GameLogic newLogic = new GameLogic(newState);
                    newLogic.ApplyMove(move);

                    int score = -AlphaBeta(newState, currentDepth - 1, -beta, -alpha, false);

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
                    else if (score == bestValue)
                    {
                        bestMove.Add(move);
                    }
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

            Int64 zHash = state.GetZobristHash();
            Move bestMove = null;

            if (transpositionTable.ContainsKey(zHash))
            {
                Transposition transposition = transpositionTable[zHash];

                bestMove = transposition.BestMove;

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
                score = -QuiescenceSearch(state, -beta, -alpha, !myMove);
            }
            else
            {
                score = int.MinValue;
                var possibleMoves = gameLogic.GetPossibleMoves();

                OrderMoves(possibleMoves, state, bestMove);
                // Reset best move
                bestMove = null;

                // Multi-Cut
                {
                    Move move = possibleMoves[0];

                    Gameboard newState = (Gameboard)state.Clone();
                    GameLogic newLogic = new GameLogic(newState);
                    newLogic.ApplyMove(move);

                    int c = 0;
                    int m = 0;

                    while (newState != null && m < 10)
                    {
                        int value = -AlphaBeta(newState, depth - 1 - 2, GameLogic.LOSS_VALUE, GameLogic.WIN_VALUE, false);

                        if (value >= beta)
                        {
                            c++;
                            if (c > 3)
                            {
                                return beta;
                            }
                        }

                        m++;

                        if (m < possibleMoves.Count)
                        {
                            move = possibleMoves[m];

                            newState = (Gameboard)state.Clone();
                            newLogic = new GameLogic(newState);
                            newLogic.ApplyMove(move);
                        }
                        else
                        {
                            newState = null;
                        }
                    }
                }

                // Calculate score for PVS
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
                        if (value >= upperbound && value < beta)
                        {
                            value = -AlphaBeta(newState, depth - 1, -beta, -value, !myMove);
                        }

                        if (value > score)
                        {
                            score = value;
                        }
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
                    Transposition transposition = new Transposition(alpha, score, depth, bestMove);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                    transposition.BestMove = bestMove;
                }
            }
            else if (score > alpha && score < beta)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(score, score, depth, bestMove);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Lowerbound = score;
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                    transposition.BestMove = bestMove;
                }
            }
            else if (score >= beta)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(score, beta, depth, bestMove);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Lowerbound = score;
                    transposition.Depth = depth;
                    transposition.BestMove = bestMove;
                }
            }

            return score;
        }

        public void OrderMoves(List<Move> moves, Gameboard state, Move bestMove = null)
        {
            // Assign values to moves, if possible
            for (int i = 0; i < moves.Count; i++)
            {
                moves[i].Value = 0;

                // Check for best move from last iteration
                if (moves[i] == bestMove)
                {
                    moves[i].Value = 1;
                    //continue;
                }

                Gameboard newState = (Gameboard)state.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(moves[i]);

                Int64 zHash = newState.GetZobristHash();
                if (transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = transpositionTable[zHash];
                    if (transposition.Lowerbound == transposition.Upperbound)
                    {
                        moves[i].Value += transposition.Lowerbound;
                    }
                }
            }
            moves.Sort(MoveComparison);
        }

        public static int MoveComparison(Move m1, Move m2)
        {
            return m1.Value.CompareTo(m2.Value);
        }

        public int QuiescenceSearch(Gameboard state, int alpha, int beta, bool myMove)
        {
            // DEBUG
            iterationCounter++;

            int score = int.MinValue;

            GameLogic gameLogic = new GameLogic(state);
            var possibleCaptureMoves = gameLogic.GetPossibleCaptureMoves();

            // If there are no capture moves possible, use evaluation function and return value
            if (possibleCaptureMoves.Count == 0)
            {
                score = myMove ? evaluation.Evaluate(state) : -evaluation.Evaluate(state);
            }
            // If there are capture moves, continue iterating
            else
            {
                foreach (Move move in possibleCaptureMoves)
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
            }

            return score;
        }

        public override void ChangePlayerType(Game.Player playerType)
        {
            base.ChangePlayerType(playerType);

            evaluation.PlayerType = playerType;
        }
    }
}
