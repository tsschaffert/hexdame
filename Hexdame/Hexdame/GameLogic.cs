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

        public void ApplyMove(Game.Player activePlayer, Move move)
        {
            if (!IsValidMove(activePlayer, move))
            {
                return;
            }

            Cell startingCell = gameboard.GetCell(move.GetStartingPosition());
            Cell moveToCell = gameboard.GetCell(move.GetPosition(1));

            Cell.Occupancy stoneToMove = startingCell.Content;
            startingCell.Content = Cell.Occupancy.Empty;
            moveToCell.Content = stoneToMove;

            Console.WriteLine("Player " + (int)activePlayer + ":");
            Console.WriteLine(gameboard);
        }

        public bool IsValidMove(Game.Player activePlayer, Move move)
        {
            return true;
        }
    }
}
