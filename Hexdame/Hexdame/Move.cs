using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class Move
    {
        List<Position> positions;

        public Move(params Position[] positions)
        {
            this.positions = new List<Position>();
            for (int i = 0; i < positions.Length; i++)
            {
                this.positions.Add(positions[i]);
            }
        }
    }
}
