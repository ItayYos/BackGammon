using Common.BackGammonModels;

namespace Server.Gameboard_Initializer
{
    public interface IGameBoardGenerator
    {
        GameBoard Generate();
    }
}
