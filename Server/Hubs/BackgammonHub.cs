using Common.BackGammonModels;
using Microsoft.AspNetCore.SignalR;
using Server.BL;
using System.Collections.ObjectModel;

namespace Server.Hubs
{
    public class BackgammonHub : Hub
    {
        private IGameBL _bl;

        public BackgammonHub(IGameBL bl)
        {
            _bl = bl;
        }
                
        public void Login(string username)
        {
            string connectionId = this.Context.ConnectionId;
            _bl.Login(username, connectionId);
        }
        
        public void Start(string opponent)
        {
            string connectionId = this.Context.ConnectionId;
            _bl.Start(connectionId, opponent);
        }

        public Collection<Move> CalcPossibleMoves(GameBoard board, string opponent)
        {
            string connectionId = this.Context.ConnectionId;
            return _bl.CalcPossibleMoves(board, connectionId, opponent);
        }

        public void EndTurn(string opponent)
        {
            string connectionId = this.Context.ConnectionId;
            _bl.EndTurn(connectionId, opponent);
        }
        public void SendDicesRoll(int dice1, int dice2, string opponent)
        {
            string usernameConnectionId = this.Context.ConnectionId;
            _bl.DiceRoll(usernameConnectionId, opponent, dice1, dice2);
        }

        public string Move(Move move, string opponent)
        {
            string connectionId = this.Context.ConnectionId;
            return _bl.Move(move, connectionId, opponent);
        }
    }
}
