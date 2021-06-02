namespace Common.BackGammonModels
{
    public class GameBoard
    {
        public GameTile[] Tiles { get; set; }
        public Dice Dice1 { get; set; }
        public Dice Dice2 { get; set; }

        public GameBoard()
        {
            Tiles = new GameTile[0];
            Dice1 = new Dice();
            Dice2 = new Dice();
        }
    }
}
