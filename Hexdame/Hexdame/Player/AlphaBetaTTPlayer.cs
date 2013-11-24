﻿using System;
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
            Transposition transposition = null;
            if(transpositionTable.ContainsKey(state.GetZobristHash()))
            {
                transposition = transpositionTable[state.GetZobristHash()];
                if(transposition.Lowerbound >= beta)
                {
                    return transposition.Lowerbound;
                }
                if(transposition.Upperbound <= alpha)
                {
                    return transposition.Upperbound;
                }
                alpha = Math.Max(alpha, transposition.Lowerbound);
                beta = Math.Min(beta, transposition.Upperbound);
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
                if (transposition == null)
                {
                    transposition = new Transposition(alpha, score, depth);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                }
            }
            else if (score > alpha && score < beta)
            {
                if (transposition == null)
                {
                    transposition = new Transposition(score, score, depth);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
                    transposition.Lowerbound = score;
                    transposition.Upperbound = score;
                    transposition.Depth = depth;
                }
            }
            else if (score >= beta)
            {
                if (transposition == null)
                {
                    transposition = new Transposition(score, beta, depth);
                    transpositionTable.Add(state.GetZobristHash(), transposition);
                }
                else
                {
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