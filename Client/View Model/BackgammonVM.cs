using Client.BL;
using Common;
using Common.BackGammonModels;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WPFClient.Commands;

namespace Client.View_Model
{
    class BackgammonVM : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _canRoll;
        private bool _canMove;
        private BackgammonBL _gameBL;
        private string _diceImgPath;
        private Move _move;

        private ObservableCollection<GameTile> _gameBoard;
        public ObservableCollection<GameTile> GameBoard
        {
            get { return _gameBoard; }
            set { _gameBoard = value; OnPropertyChanged(nameof(GameBoard)); }
        }

        private string _dice1Source;
        public string Dice1Source
        {
            get { return _dice1Source; }
            set { _dice1Source = value; OnPropertyChanged(nameof(Dice1Source)); }
        }

        private string _dice2Source;

        public string Dice2Source
        {
            get { return _dice2Source; }
            set { _dice2Source = value; OnPropertyChanged(nameof(Dice2Source)); }
        }

        private string _output;
        public string Output
        {
            get { return _output; }
            set { _output = value; OnPropertyChanged(nameof(Output)); }
        }

        public CommandExecuter RollCommand { get; set; }
        public RelayCommand<string> TileClickCommand { get; set; }
        public BackgammonVM()
        {
            _gameBL = new BackgammonBL();
            _canRoll = false;
            _canMove = false;
            _diceImgPath = "/Assets/Backgammon/";
            GameBoard = new ObservableCollection<GameTile>();
            Dice1Source = _diceImgPath + "1.png";
            Dice2Source = _diceImgPath + "1.png";
            RollCommand = new CommandExecuter(RollDice, () => true);
            TileClickCommand = new RelayCommand<string>(TileClick);
            _move = new Move {Origin = -1, Destination = -1 };
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnLoggedIn(string users)
        {
            string[] strArr = users.Split(",");
            _gameBL.ConnectToServer(strArr[0], strArr[1]);
            _gameBL.Start();
            _gameBL.GetGameboardEvent += GetGameboard;
            _gameBL.MyTurnEvent += MyTurn;
            _gameBL.GetTilesEvent += GetGameTiles;
            _gameBL.EndTurnEvent += EndTurn;
            _gameBL.GameEndedEvent += GameEnded;
        }

        private async void RollDice()
        {
            if (_canRoll)
            {
                Tuple<int, int> roll = _gameBL.RollDices();
                Dice1Source = _diceImgPath + roll.Item1.ToString() + ".png";
                Dice2Source = _diceImgPath + roll.Item2.ToString() + ".png";
                _canRoll = false;
                if (await CheckforPossibleMoves(roll.Item1, roll.Item2))
                    _canMove = true;
                else
                    EndTurn();
            }
        }

        private void EndTurn(object sender, EventArgs e)
        {
            EndTurn();
        }

        private void EndTurn()
        {
            _canMove = false;
            _canRoll = false;
            Output = "Turn ended.";
            _gameBL.EndTurn();
        }

        private async Task<bool> CheckforPossibleMoves(int dice1, int dice2)
        {
            return await _gameBL.CheckforPossibleMoves(dice1, dice2, GameBoard);
        }

        private async void TileClick(string tileIndex)
        {
            bool res = int.TryParse(tileIndex, out int index);
            if (res && _canMove)
            {
                if (_move.Origin == -1)
                {
                    _move.Origin = index;
                    return;
                }
                if (_move.Destination == -1)
                {
                    _move.Destination = index;
                    Output = await _gameBL.Move(_move);
                    ResetMove();
                }
            }
        }

        private void ResetMove()
        {
            _move.Origin = -1;
            _move.Destination = -1;
        }
        private void MyTurn(object sender, EventArgs e)
        {
            _canRoll = true;
            Output = "My turn.";
        }
        private void GetGameboard(object sender, GameEventArgs e)
        {
            GameBoard = new ObservableCollection<GameTile>(e.Gameboard.Tiles);
            if(e.Gameboard.Dice1.Value < 7 && e.Gameboard.Dice1.Value > 0)
                Dice1Source = _diceImgPath + e.Gameboard.Dice1.Value.ToString() + ".png";
            if (e.Gameboard.Dice2.Value < 7 && e.Gameboard.Dice2.Value > 0)
                Dice2Source = _diceImgPath + e.Gameboard.Dice2.Value.ToString() + ".png";
        }

        private void GetGameTiles(object sender, GameEventArgs e)
        {
            GameBoard = new ObservableCollection<GameTile>(e.GameTiles);
        }

        private void GameEnded(object sender, string e)
        {
            _canMove = false;
            _canRoll = false;
            Output = e;
        }
    }
}
