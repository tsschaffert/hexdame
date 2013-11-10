using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    abstract class AbstractHumanPlayer : AbstractPlayer
    {
        public AbstractHumanPlayer(Game.Player playerType)
            : base(playerType)
        {

        }
    }
}
