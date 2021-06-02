using Common.BackGammonModels;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class GameNotificator : IGameNotificator
    {
        private IHubContext<BackgammonHub> _userHubContext;

        public GameNotificator(IHubContext<BackgammonHub> hubContext)
        {
            _userHubContext = hubContext;
        }
        public void SendBoard(string connectionId, GameBoard gb)
        {
            _userHubContext.Clients.Client(connectionId).SendAsync("GetGameboard", gb);
        }

        public void YourTurnNotification(string connectionId)
        {
            _userHubContext.Clients.Client(connectionId).SendAsync("MyTurn");
        }
        /*
        public void SendPlayer(string connectionId, string player)
        {
            _userHubContext.Clients.Client(connectionId).SendAsync("GetPlayer", player);
        }*/

        public void GameEnded(string connectionId1, string connectionId2, string winner)
        {
            _userHubContext.Clients.Client(connectionId1).SendAsync("GameEnded", winner);
            _userHubContext.Clients.Client(connectionId2).SendAsync("GameEnded", winner);
        }

        public void SendGamePiecesOnly(string connectionId, GameTile[] tiles)
        {
            _userHubContext.Clients.Client(connectionId).SendAsync("GetTiles", tiles);
        }
    }
}
