using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Life
{
    public class GameCreator
    {

        public static LifeGame RectangleTorGame(int aHeight, int aWidth, int aItemsCount) //, int aMaxAge)
        {
            SquareCellTorGameField _sctgf = new SquareCellTorGameField(aWidth, aHeight);
            return new LifeGame(_sctgf, aItemsCount); //, aMaxAge);
        }


        public static LifeGame HexagonalTorGame(int aHeight, int aWidth, int aItemCount) //, int aMaxAge)
        {
            HexagonCellTorGameField _hctgf = new HexagonCellTorGameField(aWidth, aHeight);
            return new LifeGame(_hctgf, aItemCount); //, aMaxAge);
        }
    }
}
