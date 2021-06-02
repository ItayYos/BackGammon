
using Common.BackGammonModels;

namespace Server.Hubs
{
    public interface IGameNotificator
    {
        void SendBoard(string connectionId, GameBoard gb);
        void YourTurnNotification(string connectionId);
        //void SendPlayer(string connectionId, string player);//aubsolete.
        void GameEnded(string username, string opponent, string winner);//username and opponet are their connectionIds respectively.
        void SendGamePiecesOnly(string connectionId, GameTile[] tiles);
    }
}
