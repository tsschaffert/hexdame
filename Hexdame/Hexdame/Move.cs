using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class Move : ICloneable
    {
        private List<Position> positions;

        public int Captures
        {
            get
            {
                if (positions.Count < 2)
                {
                    return -1;
                }
                else if (positions.Count > 2)
                {
                    return positions.Count - 1;
                }
                else
                {
                    int deltaNumber = positions[1].Number - positions[0].Number;
                    int deltaCharacter = positions[1].Character - positions[0].Character;

                    // TODO won't work with kings
                    if (Math.Abs(deltaNumber) == 2 || Math.Abs(deltaCharacter) == 2)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public Move(params Position[] positions)
        {
            this.positions = new List<Position>();
            for (int i = 0; i < positions.Length; i++)
            {
                this.positions.Add(positions[i]);
            }
        }

        public Position GetStartingPosition()
        {
            return GetPosition(0);
        }

        public Position GetPosition(int index)
        {
            return positions[index];
        }

        public int GetNumberOfPositions()
        {
            return positions.Count;
        }

        public static bool operator ==(Move a, Move b)
        {
            if (a.positions.Count != b.positions.Count)
            {
                return false;
            }

            for (int i = 0; i < a.positions.Count; i++)
            {
                if (a.positions[i] != b.positions[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Move a, Move b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Move)
            {
                Move m = (Move)obj;
                return m == this;
            }
            return false;
        }

        public void InsertPosition(int index, Position item)
        {
            positions.Insert(index, item);
        }

        public void AddPosition(Position item)
        {
            positions.Add(item);
        }

        public void RemoveLastPosition()
        {
            if (positions.Count > 0)
            {
                positions.RemoveAt(positions.Count - 1);
            }
        }

        public override int GetHashCode()
        {
            return positions.Count;// TODO
        }

        public object Clone()
        {
            Move move = new Move(this.positions.ToArray());
            return move;
        }

        public bool ContainsCapture(Position capturePosition)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                Position current = positions[i];
                Position next = positions[i+1];
                Position currentCapurePosition = new Position(current.Number + (next.Number - current.Number) / 2, current.Character + (next.Character - current.Character) / 2);
                if (currentCapurePosition == capturePosition)
                {
                    return true;
                }
            }
            return false;
        }   
    }
}
