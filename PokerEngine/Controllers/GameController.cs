using PokerEngine.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PokerEngine.Controllers
{
    [RoutePrefix("api/games")]
    public class GameController : ApiController
    {
        [HttpPut, Route("")]
        public IHttpActionResult NewGame()
        {
            Game game = new Game();

            game.Log.Push(new LogEntry { Message = $"Game {game.GameId} created." });

            for (int i = 0; i < 5; i++)
            {
                game.Players.Add(new Player()
                {
                    Hole = {
                        game.Deck.Draw(),
                        game.Deck.Draw()
                    }
                });
            }

            game.Log.Push(new LogEntry { Message = $"Player {game.Players[game.DealerIndex].PlayerId} is dealer." });
            foreach (var p in game.Players)
            {
                game.Log.Push(new LogEntry { Message = $"Player {p.PlayerId} served." });
            }

            // blinds
            int sbIdx = (game.DealerIndex + 1) % game.Players.Count;
            int bbIdx = (game.DealerIndex + 2) % game.Players.Count;
            game.CurrentBet = game.BigBlind;
            game.Pot = game.SmallBlind + game.BigBlind;
            game.Players[sbIdx].Stack -= game.SmallBlind;
            game.Log.Push(new LogEntry { Message = $"Player {game.Players[bbIdx].PlayerId} has small blind." });
            game.Players[bbIdx].Stack -= game.BigBlind;
            game.Log.Push(new LogEntry { Message = $"Player {game.Players[bbIdx].PlayerId} has big blind." });

            game.CurrentIndex = (game.DealerIndex + 3) % game.Players.Count;

            Player player = game.Players[game.CurrentIndex];
            game.Log.Push(new LogEntry { Message = $"Player {player.PlayerId} is next to play." });
            game.AvailableActions = new List<PlayerAction>
            {
                new PlayerAction
                {
                    PlayerId = player.PlayerId,
                    Type = ActionType.Raise
                },
                new PlayerAction
                {
                    PlayerId = player.PlayerId,
                    Type = ActionType.Call
                },
                new PlayerAction
                {
                    PlayerId = player.PlayerId,
                    Type = ActionType.Fold
                }
            };

            GameRepository.Games.Add(game.GameId, game);

            return Ok(game);
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetGame(Guid id)
        {
            if (GameRepository.Games.TryGetValue(id, out Game game))
            {
                return Ok(game);
            }

            return NotFound();
        }

        // POST api/<controller>
        [HttpPost]
        public IHttpActionResult ApplyAction([FromBody] Guid id, [FromBody] string actionType, [FromBody] int amount)
        {
            if (!GameRepository.Games.TryGetValue(id, out Game game))
                return NotFound();

            PlayerAction action = game.AvailableActions.Find(a => a.Type.ToString() == actionType);

            if (action == null)
                return NotFound();

            // TODO: Apply action to game state

            return Ok(game);
        }
    }
}