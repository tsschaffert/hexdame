using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class SimpleMinimaxPlayer : AbstractMinimaxPlayer
    {
        public SimpleMinimaxPlayer(Game.Player playerType)
            : base(playerType, 4)
        {
        }

        public override int Evaluate(Gameboard state)
        {
            int value = 0;

            for (int i = 1; i <= Gameboard.FIELD_SIZE; i++)
            {
                for (int j = 1; j <= Gameboard.FIELD_SIZE; j++)
                {
                    Position currentPosition = new Position(i, j);

                    if (state.ValidCell(currentPosition))
                    {
                        Cell currentCell = state.GetCell(currentPosition);
                        // Player man on current position?
                        if (currentCell.ContainsRed)
                        {
                            if (playerType == Game.Player.Red)
                            {
                                value += currentCell.ContainsKing ? 3 : 1;
                            }
                            else
                            {
                                value -= currentCell.ContainsKing ? 3 : 1;
                            }
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            if (playerType == Game.Player.Red)
                            {
                                value -= currentCell.ContainsKing ? 3 : 1;
                            }
                            else
                            {
                                value += currentCell.ContainsKing ? 3 : 1;
                            }
                        }
                    }
                }
            }

            return value;
        }
    }
}
