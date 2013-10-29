using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexdame
{
    class Game
    {
        private Gameboard gameboard;
        public const int FIELD_SIZE = 0;

        public Game()
        {
            gameboard = new Gameboard();
            NewGame();
        }

        

        public void NewGame()
        {
            gameboard.Reset();

            Console.WriteLine(gameboard);
        }
    }
}
