using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    abstract class AbstractComputerPlayer : AbstractPlayer
    {
        public AbstractComputerPlayer(Game.Player playerType)
            : base(playerType)
        {

        }

        public abstract Move GetMove(Gameboard gameboard);
    }
}
