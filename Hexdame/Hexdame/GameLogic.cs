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
            List<Move> validMoves = GetPossibleMoves(activePlayer);
            return validMoves.Contains(move);

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

                    if (gameboard.ValidCell(currentPosition))
                    {
                        Cell currentCell = gameboard.GetCell(currentPosition);
                        // Player man on current position?
                        if ((currentCell.ContainsWhite && activePlayer == Game.Player.White)
                            || (currentCell.ContainsRed && activePlayer == Game.Player.Red))
                        {
                            possibleMoves.AddRange(GetPossibleMovesForPosition(activePlayer, currentPosition));
                            possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(new Move(), activePlayer, currentPosition));
                        }
                    }
                }
            }

            // Filter invalid moves
            return new List<Move>(possibleMoves.Where(position => position.GetNumberOfPositions() >= 2));
        }

        public List<Move> GetPossibleMovesForPosition(Game.Player activePlayer, Position currentPosition)
        {
            List<Move> possibleMoves = new List<Move>();

            if (!gameboard.ValidCell(currentPosition))
            {
                return possibleMoves;
            }

            Cell currentCell = gameboard.GetCell(currentPosition);
    
            // First consider normal moves
            int moveDirection = activePlayer == Game.Player.White ? 1 : -1; // Player one moves to top

            possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(activePlayer, currentPosition, moveDirection, 0));
            possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(activePlayer, currentPosition, 0, moveDirection));
            possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(activePlayer, currentPosition, moveDirection, moveDirection));

            // TODO if king   

            return possibleMoves;
        }

        public List<Move> GetPossibleCaptureMovesForPosition(Move currentMove, Game.Player activePlayer, Position currentPosition)
        {
            List<Move> possibleMoves = new List<Move>();

            if (!gameboard.ValidCell(currentPosition))
            {
                return possibleMoves;
            }

            Cell currentCell = gameboard.GetCell(currentPosition);

            currentMove.AddPosition(currentPosition);

            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, activePlayer, currentPosition, 1, 0));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, activePlayer, currentPosition, 0, 1));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, activePlayer, currentPosition, 1, 1));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, activePlayer, currentPosition, -1, 0));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, activePlayer, currentPosition, 0, -1));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, activePlayer, currentPosition, -1, -1));

            // TODO if king

            if (possibleMoves.Count == 0 && currentMove.GetNumberOfPositions() >= 2)
            {
                possibleMoves.Add((Move)currentMove.Clone());
            }

            currentMove.RemoveLastPosition();

            return possibleMoves;
        }

        public List<Move> GetPossibleMovesForPositionAndDirection(Game.Player activePlayer, Position currentPosition, int directionNumber, int directionCharacter)
        {
            List<Move> possibleMoves = new List<Move>();

            Position positionToCheck = new Position(currentPosition.Number + directionNumber, currentPosition.Character + directionCharacter);

            if (!gameboard.ValidCell(positionToCheck))
            {
                return possibleMoves;
            }

            Cell cellToCheck = gameboard.GetCell(positionToCheck);
            if (cellToCheck.IsEmpty)
            {
                // Position is valid and empty, so move is finished
                possibleMoves.Add(new Move(currentPosition, positionToCheck));
            }

            return possibleMoves;
        }

        public List<Move> GetPossibleCaptureMovesForPositionAndDirection(Move currentMove, Game.Player activePlayer, Position currentPosition, int directionNumber, int directionCharacter)
        {
            List<Move> possibleMoves = new List<Move>();

            Position possibleOpponentPosition = new Position(currentPosition.Number + directionNumber, currentPosition.Character + directionCharacter);
            Position possibleMovePosition = new Position(currentPosition.Number + 2*directionNumber, currentPosition.Character + 2*directionCharacter);

            // Check if no capture possible or capture already done
            if (!gameboard.ValidCell(possibleOpponentPosition) || !gameboard.ValidCell(possibleMovePosition) || currentMove.ContainsCapture(possibleOpponentPosition))
            {
                return possibleMoves;
            }

            Cell possibleOpponentCell = gameboard.GetCell(possibleOpponentPosition);
            Cell possibleMoveCell = gameboard.GetCell(possibleMovePosition);

            if (((activePlayer == Game.Player.White && possibleOpponentCell.ContainsRed) ||
                (activePlayer == Game.Player.Red && possibleOpponentCell.ContainsWhite)) &&
                possibleMoveCell.IsEmpty)
            {
                possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(currentMove, activePlayer, possibleMovePosition));
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
