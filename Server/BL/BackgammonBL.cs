using Common.BackGammonModels;
using Server.Gameboard_Initializer;
using Server.Hubs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Server.BL
{
    public class BackgammonBL : IGameBL
    {
        public static Dictionary<string, string> _users { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> _userIDs { get; set; } = new Dictionary<string, string>();
        public static Collection<Game> _games { get; set; } = new Collection<Game>();
        private IGameBoardGenerator _gameBoardGenerator;
        private IGameNotificator _gameNotificator;

        public BackgammonBL(IGameBoardGenerator gameBoardGenerator, IGameNotificator gameNotificator)
        {
            _gameBoardGenerator = gameBoardGenerator;
            _gameNotificator = gameNotificator;
        }
        
        public void Login(string username, string connectionId)
        {
            _users.Add(username, connectionId);
            _userIDs.Add(connectionId, username);
        }

        public void Start(string connectionId, string opponent)
        {
            string username = _userIDs[connectionId];
            if(_users.ContainsKey(username) && _users.ContainsKey(opponent))
            {
                Game game = _games.FirstOrDefault(g=>(g.Player1 == username && g.Player2 == opponent) || (g.Player1 == opponent && g.Player2 == username));
                if(game == null)
                {
                    game = new Game();
                    game.Player1 = username;
                    game.Player2 = opponent;
                    game.Winner = string.Empty;
                    game.Turn = string.Empty;
                    game.GameBoard = new GameBoard();
                    game.GameBoard = _gameBoardGenerator.Generate();
                    game.GameBoard.Dice1.Value = 1;
                    game.GameBoard.Dice2.Value = 1;
                    _games.Add(game);
                }
                DecideWhoFirst(game);
                NotfiyTurn(game);
            }
        }

        public void DiceRoll(string usernameConnectionId, string opponent, int dice1, int dice2)
        {
            string username = _userIDs[usernameConnectionId];
            Game game = _games.FirstOrDefault(g => (g.Player1 == username && g.Player2 == opponent) || (g.Player1 == opponent && g.Player2 == username));
            if (game != null)
            {
                game.GameBoard.Dice1.Value = dice1;
                game.GameBoard.Dice2.Value = dice2;
                string connectionId = _users[opponent];
                _gameNotificator.SendBoard(connectionId, game.GameBoard);
            }
        }

        private void NotfiyTurn(Game game)
        {
            string connectionId = _users[game.Turn];
            _gameNotificator.YourTurnNotification(connectionId);
            _gameNotificator.SendBoard(_users[game.Player1], game.GameBoard);
            _gameNotificator.SendBoard(_users[game.Player2], game.GameBoard);
        }
        
        private void DecideWhoFirst(Game game)
        {
            int p1Roll = new Random().Next(1, 7);
            int p2Roll = new Random().Next(1, 7);
            if (p1Roll >= p2Roll)
                game.Turn = game.Player1;
            else
                game.Turn = game.Player2;
        }

        public void EndTurn(string connectionId, string opponent)
        {
            string username = _userIDs[connectionId];
            Game game = _games.FirstOrDefault(g => (g.Player1 == username && g.Player2 == opponent) || (g.Player1 == opponent && g.Player2 == username));
            if (CheckWin(game))
                EndGame(game);
            else
            {
                game.Turn = opponent;
                NotfiyTurn(game);
            }
        }

        private void EndGame(Game game)
        {
            string message = $"Game ended, the winner is: {game.Winner}.";
            _gameNotificator.GameEnded(_users[game.Player1], _users[game.Player2], message);
        }

        public string Move(Move move, string connectionId, string opponent)
        {
            string username = _userIDs[connectionId];
            Game game = _games.FirstOrDefault(g => (g.Player1 == username && g.Player2 == opponent) || (g.Player1 == opponent && g.Player2 == username));
            if (game != null)
            {
                string currentPlayer = GetCurrentPlayer(game, username);
                return Turn(game, move, currentPlayer, connectionId, opponent);
            }
            else
                return "Game not found";
        }

        private string Turn(Game game, Move move, string currentPlayer, string connectionId, string opponent)
        {
            GameBoard gameBoard = game.GameBoard;
            if (0 > move.Origin || move.Origin > gameBoard.Tiles.Length || 0 > move.Destination || move.Destination > gameBoard.Tiles.Length)
                return "Out of bounds";

            //if currentplayer trying to move a game piece of a different player.
            if (currentPlayer != gameBoard.Tiles[move.Origin].Pieces.FirstOrDefault()?.Player)
                return "Trying to move other player piece";

            if (move.Origin == move.Destination)
                return "No zero on dices";

            //now we know current player is moving his own piece
            Collection<Move> possibleMoves = CalcPossibleMoves(gameBoard, connectionId, opponent);
            if(possibleMoves == null)
            {
                EndTurn(connectionId, opponent);
                return "No available moves.";
            }
            Move validMove = possibleMoves.FirstOrDefault(pm => pm.Origin == move.Origin && pm.Destination == move.Destination);
            if (validMove != null)
                return TakeMove(validMove, game, currentPlayer, connectionId, opponent);
            else
                return "Move not valid";
        }

        private string TakeMove(Move move, Game game, string currentPlayer, string connectionId, string opponent)
        {
            GameBoard gameBoard = game.GameBoard;
            //We know its a valid move. we need to update board accordingly.
            GamePiece gamePiece = gameBoard.Tiles[move.Origin].Pieces.FirstOrDefault(gp => gp.Player == currentPlayer);
            if (gamePiece == null)
                return $"No game piece of {currentPlayer} on origin tile";
            gameBoard.Tiles[move.Origin].Pieces.Remove(gamePiece);
            GameTile destinationTile = gameBoard.Tiles[move.Destination];
            //Checing if by moving we capture opponent piece and place it in "jail".
            if (move.Destination != 25 && destinationTile.Pieces?.Count == 1 && destinationTile.Pieces.FirstOrDefault()?.Player != currentPlayer)
            {
                GamePiece prisoner = destinationTile.Pieces?.FirstOrDefault();
                destinationTile.Pieces.Remove(prisoner);
                gameBoard.Tiles[24].Pieces.Add(prisoner);
            }
            destinationTile.Pieces.Add(gamePiece);

            //need to send board to players without dices.
            _gameNotificator.SendGamePiecesOnly(connectionId, gameBoard.Tiles);
            _gameNotificator.SendGamePiecesOnly(_users[opponent], gameBoard.Tiles);

            //need to check win after move.
            if (CheckWin(game))
                EndGame(game);

            //need to reset the dice we played, in case its not a double.
            if (gameBoard.Dice1.Value != gameBoard.Dice2.Value)
            {
                int diff = move.Destination - move.Origin;
                if (diff < 0)
                    diff *= -1;
                if (gameBoard.Dice1.Value == diff)
                    gameBoard.Dice1.Value = 0;
                else if (gameBoard.Dice2.Value == diff)
                    gameBoard.Dice2.Value = 0; 
            }

            //Might want to check possible moves here again. just to make sure we can still move.
            Collection<Move> possibleMoves = CalcPossibleMoves(gameBoard, connectionId, opponent);
            if (possibleMoves == null)
            {
                EndTurn(connectionId, opponent);
                return "Move made, turn ended due to no more possible moves.";
            }

            return "Move made, waiting for next move.";
        }

        private bool CheckWin(Game game)
        {
            bool res = false;
            GameBoard gameBoard = game.GameBoard;

            Collection<GamePiece> outGP4P1 = new Collection<GamePiece>();
            foreach (GamePiece item in gameBoard.Tiles[25].Pieces)
            {
                if (item.Player == "Player1")
                    outGP4P1.Add(item);
            }

            List<GamePiece> outGP4P2 = new List<GamePiece>(gameBoard.Tiles[25].Pieces.Where(gp => gp.Player == "Player2"));

            if (outGP4P1.Count == 15)
            {
                game.Winner = "Player1";
                return true;
            }
            if (outGP4P2.Count == 15)
            {
                game.Winner = "Player2";
                return true;
            }
            return res;
        }

        private string GetCurrentPlayer(Game game, string username)
        {
            string ans = string.Empty;
            if (game.Player1 == username)
                ans = "Player1";
            if (game.Player2 == username)
                ans = "Player2";
            return ans;
        }

        public Collection<Move> CalcPossibleMoves(GameBoard gameBoard, string connectionId, string opponent)
        {
            string currentPlayer = string.Empty;
            string username = _userIDs[connectionId];
            Game game = _games.FirstOrDefault(g => (g.Player1 == username && g.Player2 == opponent) || (g.Player1 == opponent && g.Player2 == username));
            if (game != null && game.Player1 == username)
                currentPlayer = "Player1";
            if (game != null && game.Player2 == username)
                currentPlayer = "Player2";
                
            //We need to check for prisoners.
            GamePiece gamePiece = gameBoard.Tiles[24].Pieces?.FirstOrDefault(gp => gp?.Player == currentPlayer);
            if (gamePiece != null)
            {
                return PrisonBreak(gameBoard, currentPlayer);
            }
            //Check for going home.(removing a game piece from my home, provided all my game pieces are at home already.
            Collection<Move> takeOffBoard = null;
            if (AllHome(currentPlayer, gameBoard))
            {
                takeOffBoard = RemoveFromHome(gameBoard, currentPlayer);
            }
            //No prisoners and we are not trying to remove from home.
            Collection<Move> res = new Collection<Move>();

            Collection<int> possibleStartingPoints = new Collection<int>();
            for (int i = 0; i < gameBoard.Tiles.Length; i++)
            {
                if (currentPlayer == gameBoard.Tiles[i].Pieces.FirstOrDefault()?.Player)
                    possibleStartingPoints.Add(i);
            }

            foreach (int startIndex in possibleStartingPoints)
            {
                if (currentPlayer == "Player1")
                {
                    if (1 <= gameBoard.Dice1.Value && gameBoard.Dice1.Value <= 6)
                        Validate(startIndex, startIndex + gameBoard.Dice1.Value, res, gameBoard, currentPlayer);
                    if (1 <= gameBoard.Dice2.Value && gameBoard.Dice2.Value <= 6)
                        Validate(startIndex, startIndex + gameBoard.Dice2.Value, res, gameBoard, currentPlayer);
                }
                if (currentPlayer == "Player2")
                {
                    if (1 <= gameBoard.Dice1.Value && gameBoard.Dice1.Value <= 6)
                        Validate(startIndex, startIndex - gameBoard.Dice1.Value, res, gameBoard, currentPlayer);
                    if (1 <= gameBoard.Dice2.Value && gameBoard.Dice2.Value <= 6)
                        Validate(startIndex, startIndex - gameBoard.Dice2.Value, res, gameBoard, currentPlayer);
                }
            }
            if (takeOffBoard != null)
            {
                foreach (Move move in takeOffBoard)
                {
                    res.Add(move);
                }
            }

            return res;
        }

        private Collection<Move> PrisonBreak(GameBoard gameBoard, string currentPlayer)
        {
            Collection<Move> res = new Collection<Move>();
            if (1 <= gameBoard.Dice1.Value && gameBoard.Dice1.Value <= 6)
            {
                if (currentPlayer == "Player1")
                    PrisonBreakSubCalc(gameBoard.Dice1.Value - 1, res, gameBoard, currentPlayer);
                if (currentPlayer == "Player2")
                    PrisonBreakSubCalc(24 - gameBoard.Dice1.Value, res, gameBoard, currentPlayer);
            }
            if (1 <= gameBoard.Dice2.Value && gameBoard.Dice2.Value <= 6)
            {
                if (currentPlayer == "Player1")
                    PrisonBreakSubCalc(gameBoard.Dice2.Value - 1, res, gameBoard, currentPlayer);
                if (currentPlayer == "Player2")
                    PrisonBreakSubCalc(24 - gameBoard.Dice2.Value, res, gameBoard, currentPlayer);
            }
            return res;
        }

        private void PrisonBreakSubCalc(int index, Collection<Move> res, GameBoard gameBoard, string currentPlayer)
        {
            GameTile designatedTile = gameBoard.Tiles[index];
            if (designatedTile.Pieces?.Count <= 1)
                res.Add(new Move { Origin = 24, Destination = index });
            else if (currentPlayer == designatedTile.Pieces.FirstOrDefault().Player)
                res.Add(new Move { Origin = 24, Destination = index });
        }

        private bool AllHome(string player, GameBoard gameBoard)
        {
            if (gameBoard.Tiles[24].Pieces.FirstOrDefault(gp => gp?.Player == player) != null)
                return false;

            if (player == "Player1")
            {
                for (int i = 0; i < 18; i++)
                {
                    if (gameBoard.Tiles[i].Pieces.FirstOrDefault(gp => gp?.Player == player) != null)
                        return false;
                }
            }
            else
            {
                for (int i = 6; i < 24; i++)
                {
                    if (gameBoard.Tiles[i].Pieces.FirstOrDefault(gp => gp?.Player == player) != null)
                        return false;
                }
            }

            return true;
        }

        private void Validate(int origin, int destination, Collection<Move> res, GameBoard gameBoard, string currentPlayer)
        {
            if (0 > destination || destination > 25)
                return;
            GameTile designatedTile = gameBoard.Tiles[destination];
            if (designatedTile.Pieces.Count <= 1)
                res.Add(new Move { Origin = origin, Destination = destination });
            else if (currentPlayer == designatedTile.Pieces.FirstOrDefault()?.Player)
                res.Add(new Move { Origin = origin, Destination = destination });
        }

        private Collection<Move> RemoveFromHome(GameBoard gameBoard, string currentPlayer)
        {
            Collection<Move> res = new Collection<Move>();

            if (currentPlayer == "Player1")
            {
                if (1 <= gameBoard.Dice1.Value && gameBoard.Dice1.Value <= 6)
                    for (int i = 24 - gameBoard.Dice1.Value; i < 24; i++)
                        if (gameBoard.Tiles[i].Pieces.FirstOrDefault(gp => gp.Player == currentPlayer) != null)
                        {
                            res.Add(new Move { Origin = i, Destination = 25 });
                            i = 24;
                        }
                if (1 <= gameBoard.Dice2.Value && gameBoard.Dice2.Value <= 6)
                    for (int i = 24 - gameBoard.Dice2.Value; i < 24; i++)
                        if (gameBoard.Tiles[i].Pieces.FirstOrDefault(gp => gp.Player == currentPlayer) != null)
                        {
                            res.Add(new Move { Origin = i, Destination = 25 });
                            i = 24;
                        }
            }
            else
            {
                if (1 <= gameBoard.Dice1.Value && gameBoard.Dice1.Value <= 6)
                    for (int i = gameBoard.Dice1.Value - 1; i >= 0; i--)
                        if (gameBoard.Tiles[i].Pieces.FirstOrDefault(gp => gp.Player == currentPlayer) != null)
                        {
                            res.Add(new Move { Origin = i, Destination = 25 });
                            i = -1;
                        }
                if (1 <= gameBoard.Dice2.Value && gameBoard.Dice2.Value <= 6)
                    for (int i = gameBoard.Dice2.Value - 1; i >= 0; i--)
                        if (gameBoard.Tiles[i].Pieces.FirstOrDefault(gp => gp.Player == currentPlayer) != null)
                        {
                            res.Add(new Move { Origin = i, Destination = 25 });
                            i = -1;
                        }
            }

            return res;
        }
    }
}
