﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class EvaluationPlayer : AbstractComputerPlayer
    {
        private GameLogic gameLogic;
        private Random random;

        public EvaluationPlayer(Game.Player playerType)
            : base(playerType)
        {
            random = new Random();
        }

        public override Move GetMove(Gameboard gameboard)
        {
            gameLogic = new GameLogic(gameboard);
            List<Move> possibleMoves = gameLogic.GetPossibleMoves();
            return possibleMoves[random.Next(possibleMoves.Count)];
        }
    }
}
