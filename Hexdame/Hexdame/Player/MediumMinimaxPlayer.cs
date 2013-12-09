using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class MediumMinimaxPlayer : AbstractMinimaxPlayer
    {
        protected Evaluation evaluation;

        public MediumMinimaxPlayer(Game.Player playerType, int depth)
            : base(playerType, depth)
        {
            evaluation = new Evaluation(playerType);
        }

        public override int Evaluate(Gameboard state)
        {
            return evaluation.Evaluate(state);
        }
    }
}
