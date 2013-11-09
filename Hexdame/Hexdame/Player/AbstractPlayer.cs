using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    abstract class AbstractPlayer
    {
        protected Game.Player playerType;

        public AbstractPlayer(Game.Player playerType)
        {
            this.playerType = playerType;
        }
    }
}
