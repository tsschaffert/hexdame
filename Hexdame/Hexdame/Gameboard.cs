using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    public class Gameboard : ICloneable
    {
        private Cell[][] gameboard;
        public const int FIELD_SIZE = 9;

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

            return ret;
        }
    }
}
