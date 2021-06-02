using Common.BackGammonModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Common.Gameboard_Generators
{
    public class GameboardGenerator : IGameboardGenerator
    {
        public GameBoard gameBoard { get; set; }

        public GameboardGenerator()
        {
            gameBoard = new GameBoard();
            gameBoard.Dice1 = new Dice { Value = 0, Image = string.Empty };
            gameBoard.Dice2 = new Dice { Value = 0, Image = string.Empty };
        }

        public GameBoard Generate()
        {
            gameBoard.Tiles = new GameTile[26];
            for (int i = 0; i < gameBoard.Tiles.Length; i++)
            {
                gameBoard.Tiles[i] = new GameTile();
                gameBoard.Tiles[i].Pieces = new Collection<GamePiece>();
            }
            for (int i = 0; i < 2; i++)
            {
                gameBoard.Tiles[0].Pieces.Add(new GamePiece { Player = "Player1", Color = "Blue" });
                gameBoard.Tiles[23].Pieces.Add(new GamePiece { Player = "Player2", Color = "Yellow" });
            }
            for (int i = 0; i < 3; i++)
            {
                gameBoard.Tiles[16].Pieces.Add(new GamePiece { Player = "Player1", Color = "Blue" });
                gameBoard.Tiles[7].Pieces.Add(new GamePiece { Player = "Player2", Color = "Yellow" });
            }
            for (int i = 0; i < 5; i++)
            {
                gameBoard.Tiles[11].Pieces.Add(new GamePiece { Player = "Player1", Color = "Blue" });
                gameBoard.Tiles[18].Pieces.Add(new GamePiece { Player = "Player1", Color = "Blue" });
                gameBoard.Tiles[5].Pieces.Add(new GamePiece { Player = "Player2", Color = "Yellow" });
                gameBoard.Tiles[12].Pieces.Add(new GamePiece { Player = "Player2", Color = "Yellow" });
            }

            return gameBoard;
        }
    }
}
