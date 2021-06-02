using Common.BackGammonModels;
using System;

namespace Common
{
    public class GameEventArgs : EventArgs
    {
        public GameBoard Gameboard { get; set; }
        public GameTile[] GameTiles { get; set; }
        public GameEventArgs(GameBoard gb) : base()
        {
            Gameboard = gb;
            GameTiles = null;
        }

        public GameEventArgs(GameTile[] gameTiles) : base()
        {
            GameTiles = gameTiles;
            Gameboard = null;
        }
    }
}
