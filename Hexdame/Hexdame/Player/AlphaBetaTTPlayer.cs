using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hexdame.Model;

namespace Hexdame.Player
{
    class AlphaBetaTTPlayer : AbstractComputerPlayer
    {
        protected readonly int depth;
        protected Random random;
        protected LimitedSizeDictionary<Int64, Transposition> transpositionTable;

        public AlphaBetaTTPlayer(Game.Player playerType, int depth)
            : base(playerType)
        {
            this.depth = depth;
            random = new Random();
            transpositionTable = new LimitedSizeDictionary<Int64,Transposition>();
        }

        public override Move GetMove(Gameboard gameboard)
        {
            GameLogic gameLogic = new GameLogic(gameboard);
            Move bestMove = null;
            int bestValue = int.MinValue;

            var possibleMoves = gameLogic.GetPossibleMoves();

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
            Int64 zHash = state.GetZobristHash();
            bool moveOrdering = false;
            if(transpositionTable.ContainsKey(zHash))
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
                else if (transposition.Depth >= 1)
                {
                    moveOrdering = true;
                }
            }

            GameLogic gameLogic = new GameLogic(state);
            int score;

            if (depth <= 0 || gameLogic.IsFinished())
            {
                score = myMove ? Evaluate(state) : -Evaluate(state);
            }
            else
            {
                score = int.MinValue;
                var possibleMoves = gameLogic.GetPossibleMoves();

                // TODO move ordering
                if (moveOrdering)
                {
                    OrderMoves(possibleMoves, state);
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

        public int Evaluate(Gameboard state)
        {
            int value = 0;

            // Evaluation of piece count
            int valuePieces = EvaluatePieces(state);

            value += valuePieces;
            value += random.Next(-999, 1000);

            return value;
        }

        public void OrderMoves(List<Move> moves, Gameboard state)
        {
            moves.Sort(new MoveComparer(state, transpositionTable));
        }

        public class MoveComparer : IComparer<Move>
        {
            private Gameboard state;
            private LimitedSizeDictionary<Int64, Transposition> transpositionTable;

            public MoveComparer(Gameboard state, LimitedSizeDictionary<Int64, Transposition> transpositionTable)
            {
                this.state = state;
                this.transpositionTable = transpositionTable;
            }

            public int Compare(Move x, Move y)
            {
                Gameboard newStateX = (Gameboard)state.Clone();
                GameLogic newLogicX = new GameLogic(newStateX);
                newLogicX.ApplyMove(x);

                Gameboard newStateY = (Gameboard)state.Clone();
                GameLogic newLogicY = new GameLogic(newStateY);
                newLogicY.ApplyMove(y);

                Int64 zHashX = newStateX.GetZobristHash();
                Int64 zHashY = newStateY.GetZobristHash();

                if(!transpositionTable.ContainsKey(zHashX) || !transpositionTable.ContainsKey(zHashY))
                {
                    return 0;
                }
                else
                {
                    Transposition tX = transpositionTable[zHashX];
                    Transposition tY = transpositionTable[zHashY];

                    if(tX.Lowerbound == tX.Upperbound && tY.Lowerbound == tY.Upperbound)
                    {
                        return tX.Lowerbound - tY.Lowerbound;
                    }
                    else if(tX.Lowerbound >= tY.Upperbound)
                    {
                        return 1;
                    }
                    else if(tY.Lowerbound >= tX.Upperbound)
                    {
                        return -1;
                    }

                    return 0;
                }
            }
        }

        public int EvaluatePieces(Gameboard state)
        {
            // Number of pieces on the board
            int whiteMen = 0;
            int whiteKings = 0;
            int redMen = 0;
            int redKings = 0;
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
                            if (currentCell.ContainsKing)
                            {
                                redKings++;
                            }
                            else
                            {
                                redMen++;
                            }
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            if (currentCell.ContainsKing)
                            {
                                whiteKings++;
                            }
                            else
                            {
                                whiteMen++;
                            }
                        }
                    }
                }
            }
            // Calculate value for player White
            int valuePieces = whiteKings * 1500 + whiteMen * 1000 - redKings * 1500 - redMen * 1000;
            // If red, negate
            if (playerType == Game.Player.Red)
            {
                valuePieces = -valuePieces;
            }

            // Check for win
            if ((whiteKings + whiteMen) == 0)
            {
                return playerType == Game.Player.White ? GameLogic.LOSS_VALUE : GameLogic.WIN_VALUE;
            }
            else if ((redKings + redMen) == 0)
            {
                return playerType == Game.Player.Red ? GameLogic.LOSS_VALUE : GameLogic.WIN_VALUE;
            }

            return valuePieces;
        }
    }
}
