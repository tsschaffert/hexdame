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

        public bool ApplyMove(Game.Player activePlayer, Move move)
        {
            if (!IsValidMove(activePlayer, move))
            {
                return false;
            }

            Cell startingCell = gameboard.GetCell(move.GetStartingPosition());

            Position currentPosition = move.GetStartingPosition();
            Position nextPosition;

            int indexNextPosition = 1;
            while (indexNextPosition < move.GetNumberOfPositions())
            {
                nextPosition = move.GetPosition(indexNextPosition);

                Cell nextCell = gameboard.GetCell(nextPosition);
                Cell currentCell = gameboard.GetCell(currentPosition);

                int deltaNumber = nextPosition.Number - currentPosition.Number;
                int deltaCharacter = nextPosition.Character - currentPosition.Character;

                // Capture move?
                if (Math.Abs(deltaNumber) == 2 || Math.Abs(deltaCharacter) == 2)
                { 
                    Position capturePosition = new Position(currentPosition.Number + deltaNumber / 2, currentPosition.Character + deltaCharacter / 2);
                    Cell captureCell = gameboard.GetCell(capturePosition);
                    captureCell.Content = Cell.Occupancy.Empty;
                }
                
                // Move player
                Cell.Occupancy stoneToMove = currentCell.Content;
                currentCell.Content = Cell.Occupancy.Empty;
                nextCell.Content = stoneToMove;

                indexNextPosition++;
                currentPosition = nextPosition;
            }

            // TODO Check for king

            // TODO Check for win
            
            Console.WriteLine("Player " + (int)activePlayer + ":");
            Console.WriteLine(gameboard);

            return true;
        }

        public bool IsValidMove(Game.Player activePlayer, Move move)
        {
            /*List<Move> validMoves = GetPossibleMoves(activePlayer);
            return validMoves.Contains(move);*/

            if (move.GetNumberOfPositions() <= 1)
            {
                return false;
            }

            Position startingPosition = move.GetStartingPosition();
            Cell startingCell = gameboard.GetCell(startingPosition);
            if ((startingCell.ContainsWhite && activePlayer == Game.Player.Red)
                || (startingCell.ContainsRed && activePlayer == Game.Player.White))
            {
                return false;
            }

            int indexNextPosition = 1;

            Position currentPosition = startingPosition;
            

            while (indexNextPosition < move.GetNumberOfPositions())
            {
                Position nextPosition = move.GetPosition(indexNextPosition);

                if (!gameboard.GetCell(nextPosition).IsEmpty)
                {
                    return false;
                }


                indexNextPosition++;
                currentPosition = nextPosition;
            }
            

            return true;
        }

        public List<Move> GetPossibleMoves(Game.Player activePlayer)
        {
            List<Move> possibleMoves = new List<Move>();

            for (int i = 1; i <= Gameboard.FIELD_SIZE; i++)
            {
                for (int j = 1; j <= Gameboard.FIELD_SIZE; j++)
                {
                    Position currentPosition = new Position(i, j);
                    possibleMoves.AddRange(GetPossibleMovesForPosition(activePlayer, currentPosition));
                }
            }

            return possibleMoves;
        }

        public List<Move> GetPossibleMovesForPosition(Game.Player activePlayer, Position currentPosition)
        {
            List<Move> possibleMoves = new List<Move>();

            if (!gameboard.ValidCell(currentPosition))
            {
                return possibleMoves;
            }

            Cell currentCell = gameboard.GetCell(currentPosition);

            // Player man on current position?
            if ((currentCell.ContainsWhite && activePlayer == Game.Player.White)
                || (currentCell.ContainsRed && activePlayer == Game.Player.Red))
            {
                // First consider normal moves
                int moveDirection = activePlayer == Game.Player.White ? 1 : -1; // Player one moves to top



                // TODO if king
            }

            return possibleMoves;
        }

        public int GetNumberOfCaptureMoves(Move move)
        {
            // Expects a valid move
            int captures = 0;

            Position currentPosition = move.GetStartingPosition();
            Position nextPosition;

            int indexNextPosition = 1;
            while (indexNextPosition < move.GetNumberOfPositions())
            {
                nextPosition = move.GetPosition(indexNextPosition);

                int deltaNumber = nextPosition.Number - currentPosition.Number;
                int deltaCharacter = nextPosition.Character - currentPosition.Character;

                // Capture move?
                if (Math.Abs(deltaNumber) == 2 || Math.Abs(deltaCharacter) == 2)
                {
                    captures++;
                }

                indexNextPosition++;
                currentPosition = nextPosition;
            }

            return captures;
        }
    }
}
