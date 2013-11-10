using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class RandomPlayer : AbstractComputerPlayer
    {
        private GameLogic gameLogic;
        private Random random;

        public RandomPlayer(Game.Player playerType)
            : base(playerType)
        {
            random = new Random();
        }

        public override Move GetMove(Gameboard gameboard)
        {
            gameLogic = new GameLogic(gameboard);
            List<Move> possibleMoves = gameLogic.GetPossibleMoves(playerType);
            return possibleMoves[random.Next(possibleMoves.Count)];
        }
    }
}
