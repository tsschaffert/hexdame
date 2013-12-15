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

        // DEBUG
        private int iterationCounter;
        private int n;

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

                //if(currentDepth > 1)
                {
                    OrderMoves(possibleMoves, gameboard);
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
            Move killMove = null;

            if (transpositionTable.ContainsKey(zHash))
            {
                Transposition transposition = transpositionTable[zHash];

                killMove = transposition.KillMove;

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
                    OrderMoves(possibleMoves, state, killMove);
                    // Reset kill move
                    //killMove = null;
                }

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
                        killMove = move;
                        break;
                    }
                }
            }

            if (score <= alpha)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(alpha, score, depth, killMove);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                    transposition.KillMove = killMove;
                }
            }
            else if (score > alpha && score < beta)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(score, score, depth, killMove);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Lowerbound = score;
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                    transposition.KillMove = killMove;
                }
            }
            else if (score >= beta)
            {
                if (!transpositionTable.ContainsKey(zHash))
                {
                    Transposition transposition = new Transposition(score, beta, depth, killMove);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    Transposition transposition = transpositionTable[zHash];
                    transposition.Lowerbound = score;
                    transposition.Depth = depth;
                    transposition.KillMove = killMove;
                }
            }

            return score;
        }

        public void OrderMoves(List<Move> moves, Gameboard state, Move killMove = null)
        {
            // Assign values to moves, if possible
            for (int i = 0; i < moves.Count; i++)
            {
                // Check for kill move
                /*if (moves[i] == killMove)
                {
                    moves[i].Value = GameLogic.WIN_VALUE - 1;
                    continue;
                }*/

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

        public override void ChangePlayerType(Game.Player playerType)
        {
            base.ChangePlayerType(playerType);

            evaluation.PlayerType = playerType;
        }
    }
}
