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

        public bool ApplyMove(Move move)
        {
            if (!IsValidMove(move))
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
                Position? capturePosition = CheckForCapture(currentPosition, nextPosition);
                if (capturePosition != null)
                {
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

            // Check for king
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

            // TODO Check for win

            return true;
        }

        public bool IsValidMove(Move move)
        {
            List<Move> validMoves = GetPossibleMoves();
            return validMoves.Contains(move);
        }

        public List<Move> GetPossibleMoves()
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

            // Filter invalid moves
            return new List<Move>(possibleMoves.Where(move => NumberOfCaptures(move) == maxCaptures));
        }

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

        public List<Move> GetPossibleWithoutLookingAtMaxCaptures()
        {
            List<Move> possibleMoves = new List<Move>();
            Game.Player activePlayer = gameboard.CurrentPlayer;

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
                            possibleMoves.AddRange(GetPossibleMovesForPosition(king, currentPosition));
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
    
            int moveDirection = activePlayer == Game.Player.White ? 1 : -1; // Player one moves to top

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
                    possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(currentMove, king, possibleMovePosition));
                }
            }
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
                    Position possibleMovePosition = new Position(possibleOpponentPosition.Number + directionNumber, possibleOpponentPosition.Character + directionCharacter);

                    while (gameboard.ValidCell(possibleMovePosition) && gameboard.GetCell(possibleMovePosition).IsEmpty)
                    {
                        possibleMoves.AddRange(GetPossibleCaptureMovesForPosition(currentMove, king, possibleMovePosition));

                        possibleMovePosition = new Position(possibleMovePosition.Number + directionNumber, possibleMovePosition.Character + directionCharacter);
                    }
                }
            }

            return possibleMoves;
        }

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

            return white == 0 || red == 0;
        }

        public int NumberOfCaptures(Move move)
        {
            int numberOfPositions = move.GetNumberOfPositions();
            if (numberOfPositions < 2)
            {
                return -1;
            }
            else if (numberOfPositions > 2)
            {
                return numberOfPositions - 1;
            }
            else
            {
                // TODO could use method CheckForCapture

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

        public Game.Player GetNextPlayer(Game.Player currentPlayer)
        {
            return (Game.Player)(1 - (int)currentPlayer);
        }
    }
}
