using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class GameLogic
    {
        private Gameboard gameboard;

        public GameLogic(Gameboard gameboard)
        {
            this.gameboard = gameboard;
        }

        public void ApplyMove(Move move)
        {
            if (!IsValidMove(move))
            {
                return;
            }

            Cell startingCell = gameboard.GetCell(move.GetStartingPosition());
            Cell moveToCell = gameboard.GetCell(move.GetPosition(1));

            Cell.Occupancy stoneToMove = startingCell.Content;
            startingCell.Content = Cell.Occupancy.Empty;
            moveToCell.Content = stoneToMove;

            Console.WriteLine(gameboard);
        }

        public bool IsValidMove(Move move)
        {
            return true;
        }
    }
}
