using Common.BackGammonModels;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Server.BL
{
    public interface IGameBL
    {
        void Start(string connectionId, string opponent);
        void Login(string userName, string connectionId);
        Collection<Move> CalcPossibleMoves(GameBoard gameBoard, string connectionId, string opponent);
        void EndTurn(string connectionId, string opponent);
        void DiceRoll(string username, string opponent, int dice1, int dice2);
        string Move(Move move, string connectionId, string opponent);
        //void Login(string userName, string connectionId, string opponent);
        //void Turn(GameBoard gameBoard, string opponent);
        // void TurnEnded(string opponent);
        // void GameEnded(string userName, string opponetn, string winner);
    }
}
