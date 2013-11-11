using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    public class Move : ICloneable
    {
        private List<Position> positions;

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
                int deltaNumberNext = next.Number - current.Number;
                int deltaCharacterNext = next.Character - current.Character;
                int deltaNumberCapture = capturePosition.Number - current.Number;
                int deltaCharacterCapture = capturePosition.Character - current.Character;

                // Check if capture position between two positions of move
                if (deltaNumberNext == 0)
                {
                    if ((Math.Abs(deltaCharacterCapture) < Math.Abs(deltaCharacterNext)) && (deltaCharacterCapture * deltaCharacterNext > 0) && deltaNumberCapture == 0)
                    {
                        return true;
                    }
                }
                else if (deltaCharacterNext == 0)
                {
                    if ((Math.Abs(deltaNumberCapture) < Math.Abs(deltaNumberNext)) && (deltaNumberCapture * deltaNumberNext > 0) && deltaCharacterCapture == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if ((Math.Abs(deltaNumberCapture) < Math.Abs(deltaNumberNext)) && (deltaNumberCapture * deltaNumberNext > 0) && (Math.Abs(deltaCharacterCapture) < Math.Abs(deltaCharacterNext)) && (deltaCharacterCapture * deltaCharacterNext > 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Position pos in positions)
            {
                sb.Append(pos);
                sb.Append(" -> ");
            }
            sb.Remove(sb.Length - 4, 4);
            return sb.ToString();
        }
    }
}
