using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hexdame.Model;

namespace Hexdame.Player
{
    class AlphaBetaFinalPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected LimitedSizeDictionary<Int64, Transposition> transpositionTable;
        protected Evaluation evaluation;

        public AlphaBetaFinalPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            transpositionTable = new LimitedSizeDictionary<Int64, Transposition>();
            evaluation = new Evaluation(playerType);
        }

        /// <summary>
        /// Returns a move chosen by the player
        /// </summary>
        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            List<Move> bestMove = new List<Move>();
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

            // Don't search if only one move possible
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            OrderMoves(possibleMoves, gameboard);

            int alpha = GameLogic.LOSS_VALUE;
            int beta = GameLogic.WIN_VALUE;

            // The root level is searched in this function, for other levels, the AlphaBeta function is used
            foreach (Move move in possibleMoves)
            {
                // Apply move
                Gameboard newState = (Gameboard)gameboard.Clone();
                GameLogic newLogic = new GameLogic(newState);
                newLogic.ApplyMove(move);

                // Search deeper
                int score = -AlphaBeta(newState, depth - 1, -beta, -alpha, false);

                // Only alpha value can be adjusted at root level
                if (score > alpha)
                {
                    alpha = score;
                }

                if (score > bestValue)
                {
                    // Store new best move
                    bestMove.Clear();
                    bestMove.Add(move);
                    bestValue = score;
                }
                else if (score == bestValue)
                {
                    // Add move to other currently best moves
                    bestMove.Add(move);
                }
            }

            // Return one of the best moves
            return bestMove[random.Next(bestMove.Count)];
        }

        public int AlphaBeta(Gameboard state, int depth, int alpha, int beta, bool myMove)
        {
            Int64 zHash = state.GetZobristHash();
            Move bestMove = null;

            // Check transposition table
            if (transpositionTable.ContainsKey(zHash))
            {
                Transposition transposition = transpositionTable[zHash];

                bestMove = transposition.BestMove;

                if (transposition.Depth >= depth)
                {
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

            // Evaluate
            if (depth <= 0 || gameLogic.IsFinished())
            {
                score = myMove ? evaluation.Evaluate(state) : -evaluation.Evaluate(state);
            }
            // Search deeper
            else
            {
                score = int.MinValue;
                var possibleMoves = gameLogic.GetPossibleMoves();

                OrderMoves(possibleMoves, state, bestMove);
                // Reset best move
                bestMove = null;

                foreach (Move move in possibleMoves)
                {
                    Gameboard newState = (Gameboard)state.Clone();
                    GameLogic newLogic = new GameLogic(newState);
                    newLogic.ApplyMove(move);

                    // Continue search
                    int value = -AlphaBeta(newState, depth - 1, -beta, -alpha, !myMove);

                    // Adjust alpha and beta
                    if (value > score)
                    {
                        bestMove = move;
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


            // Store values in transposition table
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
                    // Best move is slightly better rated than other moves
                    moves[i].Value = 1;
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

        public override void ChangePlayerType(Game.Player playerType)
        {
            base.ChangePlayerType(playerType);

            evaluation.PlayerType = playerType;
        }
    }
}
