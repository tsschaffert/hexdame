using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    abstract class AbstractPlayer
    {
        protected Game.Player playerType;

        public virtual void ChangePlayerType(Game.Player playerType)
        {
            this.playerType = playerType;
        }

        public AbstractPlayer(Game.Player playerType)
        {
            this.playerType = playerType;
        }
    }
}
