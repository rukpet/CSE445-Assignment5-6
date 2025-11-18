using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace WebApplication1
{
    public partial class PlayerDeckView : UserControl
    {
        public void RenderFromJson(string gameJson)
        {
            pnlError.Visible = false;
            pnlContent.Visible = false;

            if (string.IsNullOrWhiteSpace(gameJson))
            {
                ShowErrorMessage("No game data provided.");
                return;
            }

            JObject parsed;
            try
            {
                parsed = JObject.Parse(gameJson);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Invalid game JSON: {ex.Message}");
                return;
            }

            litGameId.Text = HttpUtility.HtmlEncode(parsed.Value<string>("GameId") ?? "unknown");
            litStage.Text = HttpUtility.HtmlEncode(parsed.Value<string>("Stage") ?? "?");
            litPot.Text = parsed.Value<int?>("Pot")?.ToString() ?? "0";
            litCurrentBet.Text = parsed.Value<int?>("CurrentBet")?.ToString() ?? "0";

            var boardCards = parsed["Board"] as JArray;
            if (boardCards != null && boardCards.Count > 0)
            {
                litBoard.Text = string.Join(", ", boardCards.Select(FormatCard));
            }
            else
            {
                litBoard.Text = "(empty)";
            }

            var players = parsed["Players"] as JArray;
            int currentIndex = parsed.Value<int?>("CurrentIndex") ?? 0;

            if (players != null && players.Count > 0)
            {
                StringBuilder playersSb = new StringBuilder();
                for (int i = 0; i < players.Count; i++)
                {
                    var player = players[i];
                    string playerId = HttpUtility.HtmlEncode(player.Value<string>("PlayerId") ?? "unknown");
                    bool folded = player.Value<bool?>("Folded") ?? false;

                    playersSb.Append("<div style=\"margin-top:4px;\">");
                    playersSb.Append(i == currentIndex ? "<strong>Current player</strong>: " : "Player: ");
                    playersSb.Append(playerId);

                    if (folded)
                    {
                        playersSb.Append(" (folded)");
                    }

                    var hole = player["Hole"] as JArray;
                    if (i == currentIndex && hole != null && hole.Count > 0)
                    {
                        playersSb.Append(" — Hand: ");
                        playersSb.Append(string.Join(", ", hole.Select(FormatCard)));
                    }
                    else
                    {
                        playersSb.Append(" — Hand: [hidden]");
                    }

                    playersSb.Append("</div>");
                }

                litPlayers.Text = playersSb.ToString();
            }
            else
            {
                litPlayers.Text = "No players found.";
            }

            pnlContent.Visible = true;
        }

        private static string FormatCard(JToken token)
        {
            if (token == null)
                return "?";

            string rank = HttpUtility.HtmlEncode(token.Value<string>("Rank") ?? "?");
            string suit = HttpUtility.HtmlEncode(token.Value<string>("Suit") ?? "?");
            return $"{rank} of {suit}";
        }

        public void ShowErrorMessage(string message)
        {
            litError.Text = Server.HtmlEncode(message);
            pnlContent.Visible = false;
            pnlError.Visible = true;
        }
    }
}
