﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    [Serializable()]  
    public class Gameboard : ICloneable, ISerializable
    {
        private Cell[][] gameboard;
        private Game.Player currentPlayer;
        public const int FIELD_SIZE = 9;

        private static Int64[,] zobristRandomPositionValues;
        private static Int64[] zobristRandomPlayerValues;

        public Game.Player CurrentPlayer 
        { 
            set { currentPlayer = value; } 
            get { return currentPlayer; } 
        }

        static Gameboard()
        {
            InitialiseZobristValues();
        }

        public Gameboard()
        {
            gameboard = new Cell[FIELD_SIZE][];
            for (int i = 0; i < gameboard.Length; i++)
            {
                int numberOfCells = FIELD_SIZE - Math.Abs(FIELD_SIZE / 2 - i);
                gameboard[i] = new Cell[numberOfCells];

                for (int j = 0; j < numberOfCells; j++)
                {
                    gameboard[i][j] = new Cell(Cell.Occupancy.Empty);
                }
            }

            Reset();
        }

        public Gameboard(SerializationInfo info, StreamingContext ctxt)
        {
            currentPlayer = (Game.Player)info.GetValue("CurrentPlayer", typeof(Game.Player));
            gameboard = (Cell[][])info.GetValue("Gameboard", typeof(Cell[][]));
        }

        public Cell GetCell(Position position)
        {
            if (!ValidCell(position))
            {
                throw new IndexOutOfRangeException();
            }

            int x = position.GetArrayIndexX();
            int y = position.GetArrayIndexY();

            return gameboard[x][y];
        }

        public bool ValidCell(Position position)
        {
            int x = position.GetArrayIndexX();
            int y = position.GetArrayIndexY();

            if (x >= gameboard.Length || x < 0 || y >= gameboard[x].Length || y < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Reset()
        {
            for (int i = 1; i <= FIELD_SIZE; i++)
            {
                for (int j = 1; j <= FIELD_SIZE; j++)
                {
                    Position position = new Position(i, j);
                    if(ValidCell(position))
                    {
                        if (i <= 4 && j <= 4)
                        {
                            GetCell(position).Content = Cell.Occupancy.White;
                        }
                        else if (i >= FIELD_SIZE - 3 && j >= FIELD_SIZE - 3)
                        {
                            GetCell(position).Content = Cell.Occupancy.Red;
                        }
                        else
                        {
                            GetCell(position).Content = Cell.Occupancy.Empty;
                        }
                    }
                }
            }

            CurrentPlayer = Game.Player.White;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            List<int> entries = new List<int>();
            const int MAX = 5;

            //Count entries for each row
            for (int sum = 18; sum >= 0; sum--)
            {
                int counter = 0;
                for (int i = 1; i <= FIELD_SIZE; i++)
                {
                    Position position = new Position(i, sum - i);
                    if (ValidCell(position))
                    {
                        counter++;
                    }
                }
                entries.Add(counter);
            }

            //Build String
            for (int sum = 18; sum >= 2; sum--)
            {
                //Add Spaces
                for (int i = 0; i < MAX - entries[18 - sum]; i++)
                {
                    sb.Append(" ");
                }

                for (int i = 1; i <= FIELD_SIZE; i++)
                {
                    Position position = new Position(i, sum - i);
                    if (ValidCell(position))
                    {
                        sb.Append(GetCell(position));
                        sb.Append(" ");
                    }
                }
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public object Clone()
        {
            Gameboard ret = new Gameboard();

            for (int i = 0; i < gameboard.Length; i++)
            {
                for (int j = 0; j < gameboard[i].Length; j++)
                {
                    ret.gameboard[i][j] = (Cell)this.gameboard[i][j].Clone();
                }
            }
            ret.CurrentPlayer = CurrentPlayer;

            return ret;
        }

        private static void InitialiseZobristValues()
        {
            // Generate some random values
            Random random = new Random();

            zobristRandomPositionValues = new Int64[FIELD_SIZE*FIELD_SIZE,6];
            for (int i = 0; i < zobristRandomPositionValues.GetLength(0); i++)
            {
                for (int j = 0; j < zobristRandomPositionValues.GetLength(1); j++)
                {
                    zobristRandomPositionValues[i, j] = GenerateRandomLong(random);
                }
            }

            zobristRandomPlayerValues = new Int64[2];
            zobristRandomPlayerValues[(int)Game.Player.White] = GenerateRandomLong(random);
            zobristRandomPlayerValues[(int)Game.Player.Red] = GenerateRandomLong(random);
        }

        private static Int64 GenerateRandomLong(Random rand)
        {
            byte[] buffer = new byte[8];
            rand.NextBytes(buffer);
            Int64 randLong = BitConverter.ToInt64(buffer, 0);

            return randLong;
        }

        public Int64 GetZobristHash()
        {
            Int64 hash = 0;
            for (int i = 0; i < gameboard.Length; i++)
            {
                for (int j = 0; j < gameboard[i].Length; j++)
                {
                    hash ^= Gameboard.zobristRandomPositionValues[i * FIELD_SIZE + j, (int)gameboard[i][j].Content];
                }
            }
            hash ^= Gameboard.zobristRandomPlayerValues[(int)CurrentPlayer];
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is Gameboard)
            {
                Gameboard other = (Gameboard)obj;
                for (int i = 0; i < gameboard.Length; i++)
                {
                    for (int j = 0; j < gameboard[i].Length; j++)
                    {
                        if (gameboard[i][j].Content != other.gameboard[i][j].Content)
                        {
                            return false;
                        }
                    }
                }
                return this.CurrentPlayer==other.CurrentPlayer;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)GetZobristHash();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CurrentPlayer", currentPlayer, typeof(Game.Player));
            info.AddValue("Gameboard", gameboard, typeof(Cell[][]));
        }
    }
}
