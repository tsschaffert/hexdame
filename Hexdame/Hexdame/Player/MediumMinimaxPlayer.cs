using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame.Player
{
    class MediumMinimaxPlayer : AbstractMinimaxPlayer
    {
        public MediumMinimaxPlayer(Game.Player playerType)
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
                                value += currentCell.ContainsKing ? 30 : 10;
                            }
                            else
                            {
                                value -= currentCell.ContainsKing ? 30 : 10;
                            }
                        }
                        else if (currentCell.ContainsWhite)
                        {
                            if (playerType == Game.Player.Red)
                            {
                                value -= currentCell.ContainsKing ? 30 : 10;
                            }
                            else
                            {
                                value += currentCell.ContainsKing ? 30 : 10;
                            }
                        }
                    }
                }
            }

            value += random.Next(-9, 10);

            return value;
        }
    }
}
