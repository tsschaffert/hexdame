using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class GameLogic
    {
        // int.MAX_VALUE cannot be used because of asymmetry when negated
        public const int WIN_VALUE = 1000000;
        public const int LOSS_VALUE = -WIN_VALUE;

        private Gameboard gameboard;

        public GameLogic(Gameboard gameboard)
        {
            this.gameboard = gameboard;
        }

        /// <summary>
        /// Applies the given move, if possible. Switches the current player on success.
        /// </summary>
        public bool ApplyMove(Move move)
        {
            // Check if valid
            if (!IsValidMove(move))
            {
                return false;
            }

            // Start at first position in move
            Cell startingCell = gameboard.GetCell(move.GetStartingPosition());

            Position currentPosition = move.GetStartingPosition();
            Position nextPosition;

            int indexNextPosition = 1;
            // Iterate through all positions in the move, make capures if necessary
            while (indexNextPosition < move.GetNumberOfPositions())
            {
                nextPosition = move.GetPosition(indexNextPosition);

                Cell nextCell = gameboard.GetCell(nextPosition);
                Cell currentCell = gameboard.GetCell(currentPosition);

                int deltaNumber = nextPosition.Number - currentPosition.Number;
                int deltaCharacter = nextPosition.Character - currentPosition.Character;

                // Capture move?
                Position? capturePosition = CheckForCapture(currentPosition, nextPosition);
                if (capturePosition != null)
                {
                    // yes
                    Cell captureCell = gameboard.GetCell((Position)capturePosition);
                    captureCell.Content = Cell.Occupancy.Empty;
                }
                
                // Move player
                Cell.Occupancy stoneToMove = currentCell.Content;
                currentCell.Content = Cell.Occupancy.Empty;
                nextCell.Content = stoneToMove;

                indexNextPosition++;
                currentPosition = nextPosition;
            }

            // Check for promotion
            Position lastPosition = move.GetPosition(move.GetNumberOfPositions() - 1);
            int kingRow = gameboard.CurrentPlayer == Game.Player.White ? 9 : 1;
            if (lastPosition.Number == kingRow || lastPosition.Character == kingRow)
            {
                if (!gameboard.GetCell(lastPosition).ContainsKing)
                {
                    gameboard.GetCell(lastPosition).PromoteToKing();
                }
            }

            // Switch Player
            gameboard.CurrentPlayer = GetNextPlayer(gameboard.CurrentPlayer);

            return true;
        }

        public bool IsValidMove(Move move)
        {
            List<Move> validMoves = GetPossibleMoves();
            return validMoves.Contains(move);
        }

        /// <summary>
        /// Returns all possible moves (only those with most captures)
        /// </summary>
        public List<Move> GetPossibleMoves()
        {
            // Get all moves unfiltered
            List<Move> possibleMoves = GetPossibleWithoutLookingAtMaxCaptures();

            // Get max number of captures
            int maxCaptures = 0;
            foreach (Move move in possibleMoves)
            {
                int captures = NumberOfCaptures(move);
                if (captures > maxCaptures)
                {
                    maxCaptures = captures;
                }
            }

            // Filter invalid moves
            return new List<Move>(possibleMoves.Where(move => NumberOfCaptures(move) == maxCaptures));
        }

        /// <summary>
        /// Returns only possible capture moves (only those with most captures)
        /// </summary>
        /// <returns></returns>
        public List<Move> GetPossibleCaptureMoves()
        {
            List<Move> possibleMoves = GetPossibleWithoutLookingAtMaxCaptures();

            // Get max number of captures
            int maxCaptures = 0;
            foreach (Move move in possibleMoves)
            {
                int captures = NumberOfCaptures(move);
                if (captures > maxCaptures)
                {
                    maxCaptures = captures;
                }
            }

            // Only return list with capture moves, if no capture move, return empty list
            if(maxCaptures == 0)
            {
                return new List<Move>();
            }

            // Filter invalid moves
            return new List<Move>(possibleMoves.Where(move => NumberOfCaptures(move) == maxCaptures));
        }

        /// <summary>
        /// Returns all possible moves
        /// </summary>
        public List<Move> GetPossibleWithoutLookingAtMaxCaptures()
        {
            List<Move> possibleMoves = new List<Move>();
            Game.Player activePlayer = gameboard.CurrentPlayer;

            // Iterate over game board
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
                            bool king = currentCell.ContainsKing;
                            // Get all non-capture moves
                            possibleMoves.AddRange(GetPossibleMovesForPosition(king, currentPosition));
                            // Get all capture moves (recursivly)
                            possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(new Move(), king, currentPosition));
                        }
                    }
                }
            }

            return possibleMoves;
        }

        public List<Move> GetPossibleMovesForPosition(bool king, Position currentPosition)
        {
            List<Move> possibleMoves = new List<Move>();
            Game.Player activePlayer = gameboard.CurrentPlayer;

            if (!gameboard.ValidCell(currentPosition))
            {
                return possibleMoves;
            }

            Cell currentCell = gameboard.GetCell(currentPosition);
    
            int moveDirection = activePlayer == Game.Player.White ? 1 : -1; // Player one moves to top, player two to bottom

            possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(king, currentPosition, moveDirection, 0));
            possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(king, currentPosition, 0, moveDirection));
            possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(king, currentPosition, moveDirection, moveDirection));
 
            if (king)
            {
                // Test in every direction
                moveDirection *= -1;

                possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(king, currentPosition, moveDirection, 0));
                possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(king, currentPosition, 0, moveDirection));
                possibleMoves.AddRange(GetPossibleMovesForPositionAndDirection(king, currentPosition, moveDirection, moveDirection));
            }

            return possibleMoves;
        }

        public List<Move> GetPossibleCaptureMovesForPosition(Move currentMove, bool king, Position currentPosition)
        {
            List<Move> possibleMoves = new List<Move>();
            Game.Player activePlayer = gameboard.CurrentPlayer;

            if (!gameboard.ValidCell(currentPosition))
            {
                return possibleMoves;
            }

            Cell currentCell = gameboard.GetCell(currentPosition);

            currentMove.AddPosition(currentPosition);

            // Captures can be made in every direction
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, king, currentPosition, 1, 0));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, king, currentPosition, 0, 1));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, king, currentPosition, 1, 1));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, king, currentPosition, -1, 0));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, king, currentPosition, 0, -1));
            possibleMoves.AddRange(GetPossibleCaptureMovesForPositionAndDirection(currentMove, king, currentPosition, -1, -1));

            // If no more moves possible, return current move
            if (possibleMoves.Count == 0 && currentMove.GetNumberOfPositions() >= 2)
            {
                possibleMoves.Add((Move)currentMove.Clone());
            }

            currentMove.RemoveLastPosition();

            return possibleMoves;
        }

        public List<Move> GetPossibleMovesForPositionAndDirection(bool king, Position currentPosition, int directionNumber, int directionCharacter)
        {
            List<Move> possibleMoves = new List<Move>();
            Game.Player activePlayer = gameboard.CurrentPlayer;

            Position positionToCheck = new Position(currentPosition.Number + directionNumber, currentPosition.Character + directionCharacter);

            // Check if target position is empty. King may move any distance in the direction
            while (gameboard.ValidCell(positionToCheck) && gameboard.GetCell(positionToCheck).IsEmpty)
            {
                possibleMoves.Add(new Move(currentPosition, positionToCheck));

                positionToCheck = new Position(positionToCheck.Number + directionNumber, positionToCheck.Character + directionCharacter);

                // Man can only move one field forward if not king
                if (!king)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        public List<Move> GetPossibleCaptureMovesForPositionAndDirection(Move currentMove, bool king, Position currentPosition, int directionNumber, int directionCharacter)
        {
            List<Move> possibleMoves = new List<Move>();
            Game.Player activePlayer = gameboard.CurrentPlayer;

            // Non-king captures can only be at a distance of 2.
            if (!king)
            {
                Position possibleOpponentPosition = new Position(currentPosition.Number + directionNumber, currentPosition.Character + directionCharacter);
                Position possibleMovePosition = new Position(currentPosition.Number + 2 * directionNumber, currentPosition.Character + 2 * directionCharacter);

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
                    // Search recurivly for more captures
                    possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(currentMove, king, possibleMovePosition));
                }
            }
            // A king can move any distance
            else
            {
                Position possibleOpponentPosition = new Position(currentPosition.Number + directionNumber, currentPosition.Character + directionCharacter);
                bool isValidOpponentPosition = false;
                while (gameboard.ValidCell(possibleOpponentPosition))
                {
                    Cell possibleOpponentCell = gameboard.GetCell(possibleOpponentPosition);
                    // Look for first non-empty cell
                    if (!possibleOpponentCell.IsEmpty)
                    {
                        if(((activePlayer == Game.Player.White && possibleOpponentCell.ContainsRed) ||
                            (activePlayer == Game.Player.Red && possibleOpponentCell.ContainsWhite)) &&
                            !currentMove.ContainsCapture(possibleOpponentPosition))
                        {
                            // Move can only be valid if capture is not already made
                            isValidOpponentPosition = true;
                        }

                        break;
                    }

                    possibleOpponentPosition = new Position(possibleOpponentPosition.Number + directionNumber, possibleOpponentPosition.Character + directionCharacter);
                }
                if (isValidOpponentPosition)
                {
                    // Found an opponent position, search movement positions (king can land on any field behind an opponent)
                    Position possibleMovePosition = new Position(possibleOpponentPosition.Number + directionNumber, possibleOpponentPosition.Character + directionCharacter);

                    while (gameboard.ValidCell(possibleMovePosition) && gameboard.GetCell(possibleMovePosition).IsEmpty)
                    {
                        // Found a possible capture move, search recursivly for more captures
                        possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(currentMove, king, possibleMovePosition));

                        possibleMovePosition = new Position(possibleMovePosition.Number + directionNumber, possibleMovePosition.Character + directionCharacter);
                    }
                }
            }

            return possibleMoves;
        }

        /// <summary>
        /// Check if the board is empty or the current player cannot move anymore
        /// </summary>
        public bool IsFinished()
        {
            int red = 0;
            int white = 0;

            for (int i = 1; i <= Gameboard.FIELD_SIZE; i++)
            {
                for (int j = 1; j <= Gameboard.FIELD_SIZE; j++)
                {
                    Position currentPosition = new Position(i, j);

                    if (gameboard.ValidCell(currentPosition))
                    {
                        Cell currentCell = gameboard.GetCell(currentPosition);
                        // Player man on current position?
                        if (currentCell.ContainsRed)
                        {
                            red++;
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            white++;
                        }
                    }
                }
            }

            bool boardEmpty = white == 0 || red == 0;

            return boardEmpty || GetPossibleMoves().Count == 0;
        }

        /// <summary>
        /// Returns the number of captures for a move.
        /// </summary>
        public int NumberOfCaptures(Move move)
        {
            int numberOfPositions = move.GetNumberOfPositions();
            if (numberOfPositions < 2)
            {
                // Invalid move
                return -1;
            }
            else if (numberOfPositions > 2)
            {
                return numberOfPositions - 1;
            }
            else
            {
                // A move with two positions can have one or zero captures
                Position start = move.GetPosition(0);
                Position end = move.GetPosition(1);
                int deltaNumber = end.Number - start.Number;
                int deltaCharacter = end.Character - start.Character;

                if (Math.Abs(deltaCharacter) <= 1 && Math.Abs(deltaNumber) <= 1)
                {
                    return 0;
                }

                // Determine the direction in which the player moved
                int moveDirectionNumber = deltaNumber / Math.Max(Math.Abs(deltaNumber), Math.Abs(deltaCharacter));
                int moveDirectionCharacter = deltaCharacter / Math.Max(Math.Abs(deltaNumber), Math.Abs(deltaCharacter));

                Position newPosition = new Position(start.Number + moveDirectionNumber, start.Character + moveDirectionCharacter);
                while (newPosition != end)
                {
                    // If there is a man between the two positions, the move captures a man
                    if (!gameboard.GetCell(newPosition).IsEmpty)
                    {
                        return 1;
                    }

                    newPosition = new Position(newPosition.Number + moveDirectionNumber, newPosition.Character + moveDirectionCharacter);
                }
                return 0;
            }
        }

        /// <summary>
        /// Returns the position on which an opponent is captured.
        /// </summary>
        public Position? CheckForCapture(Position start, Position end)
        {
            int deltaNumber = end.Number - start.Number;
            int deltaCharacter = end.Character - start.Character;

            if (Math.Abs(deltaCharacter) <= 1 && Math.Abs(deltaNumber) <= 1)
            {
                return null;
            }

            // Determine the direction in which the player moved
            int moveDirectionNumber = deltaNumber / Math.Max(Math.Abs(deltaNumber), Math.Abs(deltaCharacter));
            int moveDirectionCharacter = deltaCharacter / Math.Max(Math.Abs(deltaNumber), Math.Abs(deltaCharacter));

            Position newPosition = new Position(start.Number + moveDirectionNumber, start.Character + moveDirectionCharacter);
            while (newPosition != end)
            {
                // If there is a man between the two positions, the move captures a man
                if ((gameboard.GetCell(newPosition).ContainsWhite && gameboard.CurrentPlayer == Game.Player.Red)
                    || (gameboard.GetCell(newPosition).ContainsRed && gameboard.CurrentPlayer == Game.Player.White))
                {
                    return newPosition;
                }

                newPosition = new Position(newPosition.Number + moveDirectionNumber, newPosition.Character + moveDirectionCharacter);
            }
            return null;
        }

        public static Game.Player GetNextPlayer(Game.Player currentPlayer)
        {
            return (Game.Player)(1 - (int)currentPlayer);
        }
    }
}
