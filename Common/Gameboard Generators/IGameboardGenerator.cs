using Common.BackGammonModels;

namespace Common.Gameboard_Generators
{
    public interface IGameboardGenerator
    {
        GameBoard Generate();
    }
}
