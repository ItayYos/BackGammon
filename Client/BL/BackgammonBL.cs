using Common;
using Common.BackGammonModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Client.BL
{
    class BackgammonBL
    {
        public event EventHandler MyTurnEvent;
        public event EventHandler<GameEventArgs> GetGameboardEvent;
        public event EventHandler<GameEventArgs> GetTilesEvent;
        public event EventHandler EndTurnEvent;
        public event EventHandler<string> GameEndedEvent;
        private HubConnection _hubConnection;
        private string _opponent;
        private int _movesCounter;
        public BackgammonBL()
        {
            _opponent = string.Empty;
            _movesCounter = 0;
        }

        public void ConnectToServer(string username, string opponent)
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:56556/Backgammon")
            .Build();

            _hubConnection.On("GetGameboard", async (GameBoard gb) => {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        GetGameboardEvent(this, new GameEventArgs(gb));
                    });
            });
            _hubConnection.On("MyTurn", async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MyTurnEvent(this, new EventArgs());
                });
            });
            _hubConnection.On("GetTiles", async (GameTile[] gameTiles) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GetTilesEvent(this, new GameEventArgs(gameTiles));
                });
            });
            _hubConnection.On("GameEnded", async (string message) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    GameEndedEvent(this, message);
                });
            });

            _hubConnection.StartAsync();
            _hubConnection.InvokeAsync("Login", username);
            _opponent = opponent;
        }

        public void Start()
        {
            _hubConnection.InvokeAsync("Start", _opponent);
        }
        
        public Tuple<int, int> RollDices()
        {
            int dice1 = new Random().Next(1, 7);
            int dice2 = new Random().Next(1, 7);
            Tuple<int, int> ans = new Tuple<int, int>(dice1, dice2);
            _hubConnection.InvokeAsync("SendDicesRoll", dice1, dice2, _opponent);
            return ans;
        }

        public async Task<string> Move(Move move)
        {
            string ans = await _hubConnection.InvokeAsync<string>("Move", move, _opponent);
            if (ans == "Move made, waiting for next move.")
                UpdateMovesCounter();
            return ans;
        }
        public async Task<bool> CheckforPossibleMoves(int dice1, int dice2, ObservableCollection<GameTile> board)
        {
            GameBoard gameboard = new GameBoard();
            gameboard.Tiles = board.ToArray();
            gameboard.Dice1.Value = dice1;
            gameboard.Dice2.Value = dice2;
            Collection<Move> ans = await _hubConnection.InvokeAsync<Collection<Move>>("CalcPossibleMoves", gameboard, _opponent);
            if (ans != null)
            {
                SetMovesCounter(dice1, dice2);
                return true;
            }
            else
                return false;
        }
        
        private void SetMovesCounter(int dice1, int dice2)
        {
            if (dice1 == dice2)
                _movesCounter = 4;
            else
                _movesCounter = 2;
        }

        private void UpdateMovesCounter()
        {
            _movesCounter -= 1;
            if (_movesCounter == 0)
                EndTurnEvent(this, new EventArgs());
        }

        public void EndTurn()
        {
            _hubConnection.InvokeAsync("EndTurn", _opponent);
        }
    }
}
