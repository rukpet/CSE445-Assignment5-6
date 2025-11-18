using LocalComponents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using WebApplication1.ServiceReference1;
using WebApplication1.PokerBotServiceReference;

namespace WebApplication1
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindServiceDirectory();
        }

        // Member / Staff navigation with auth check
        protected void btnMember_Click(object sender, EventArgs e)
        {
            NavigateProtected("~/Member.aspx");
        }

        protected void btnStaff_Click(object sender, EventArgs e)
        {
            NavigateProtected("~/Staff.aspx");
        }

        private void NavigateProtected(string protectedUrl)
        {
            if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect(protectedUrl, endResponse: false);
            }
            else
            {
                string loginUrl = "~/Login.aspx";
                string returnUrl = HttpUtility.UrlEncode(VirtualPathUtility.ToAbsolute(protectedUrl));
                Response.Redirect($"{loginUrl}?returnUrl={returnUrl}", endResponse: false);
            }
        }

        // Directory rows
        private class DirectoryRow
        {
            public string Provider { get; set; }
            public string ComponentType { get; set; }
            public string Operation { get; set; }
            public string Parameters { get; set; }
            public string ReturnType { get; set; }
            public string Description { get; set; }
            public string TryItAnchor { get; set; } // #anchor in this page
        }

        private void BindServiceDirectory()
        {
            List<DirectoryRow> rows = new List<DirectoryRow>
            {
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "Poker apply action",
                    Parameters = "gameId: guid, actionType: string, amount: int",
                    ReturnType = "string (game JSON)",
                    Description = "Submits a player action to the poker engine",
                    TryItAnchor = "#tryitPokerApplyAction"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "WSDL (WCF)",
                    Operation = "Poker bot decision",
                    Parameters = "gameState: json",
                    ReturnType = "BotDecisionResponse",
                    Description = "Calls Gemini via WCF to suggest the next poker action",
                    TryItAnchor = "#tryitPokerBot"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "WSDL (WCF)",
                    Operation = "WebDownload(url: string)",
                    Parameters = "url: string",
                    ReturnType = "string",
                    Description = "Fetches raw HTML/text from the URL",
                    TryItAnchor = "#tryitWebDownload"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "wordfilter(text: string)",
                    Parameters = "text: string",
                    ReturnType = "string (filtered words)",
                    Description = "Removes tags/stopwords; returns tokens",
                    TryItAnchor = "#tryitWordFilter"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog add",
                    Parameters = "category: string, item: string",
                    ReturnType = "string",
                    Description = "Adds key/value to JSON catalog",
                    TryItAnchor = "#tryitCatalogAdd"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog delete",
                    Parameters = "category: string, item: string",
                    ReturnType = "string",
                    Description = "Deletes key/value from JSON catalog",
                    TryItAnchor = "#tryitCatalogDelete"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog list all",
                    Parameters = "none",
                    ReturnType = "string (all catalog items)",
                    Description = "Lists all category/item pairs in JSON catalog",
                    TryItAnchor = "#tryitCatalogList"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog get item",
                    Parameters = "category: string, item: string",
                    ReturnType = "string (confirmation if found)",
                    Description = "Gets a specific category/item pair from JSON catalog",
                    TryItAnchor = "#tryitCatalogGet"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "cart refresh",
                    Parameters = "none",
                    ReturnType = "string (cart items)",
                    Description = "Displays all items currently in the shopping cart",
                    TryItAnchor = "#tryitCart"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "cart checkout",
                    Parameters = "none",
                    ReturnType = "string (thank you message)",
                    Description = "Processes checkout, shows thank you message, validate shipping address via 3rd party API and removes items from catalog",
                    TryItAnchor = "#tryitCart"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "DLL",
                    Operation = "Encrypt",
                    Parameters = "string input",
                    ReturnType = "string",
                    Description = "Local component providing Base64 encryption",
                    TryItAnchor = "#tryitDllEncrypt"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "DLL",
                    Operation = "Decrypt",
                    Parameters = "string base64",
                    ReturnType = "string",
                    Description = "Local component providing Base64 decryption",
                    TryItAnchor = "#tryitDllDecrypt"
                }
            };

            gvDirectory.DataSource = rows;
            gvDirectory.DataBind();
        }

        // TryIt handlers
        private BotDecisionResponse RequestPokerBot(string gameStateJson)
        {
            BotRequest request = new BotRequest { GameStateJson = gameStateJson };
            BotDecisionResponse response = new PokerBotServiceClient().GetBotDecision(request);

            return response;
        }

        private string DoGet(string url)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                try { return wc.DownloadString(url); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string DoPut(string url, string body)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                try { return wc.UploadString(url, "PUT", body ?? string.Empty); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string DoJson(string url, string body)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                try { return wc.UploadString(url, "POST", body ?? string.Empty); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string ReadError(WebException ex)
        {
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)ex.Response)
                using (System.IO.Stream stream = resp.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch { return ex.Message; }
        }

        protected void btnNewGame_Click(object sender, EventArgs e)
        {
            string result = DoPut("https://localhost:44335/api/games/", "");
            litPoker.Text = JToken.Parse(result).ToString(Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "<br/>");
        }

        protected void btnPokerApplyAction_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerGameId.Text, out Guid gameId))
            {
                litPokerApplyActionResult.Text = "Invalid game id.";
                return;
            }

            int amount = 0;
            if (!string.IsNullOrWhiteSpace(txtPokerAmount.Text) && !int.TryParse(txtPokerAmount.Text, out amount))
            {
                litPokerApplyActionResult.Text = "Amount must be a number.";
                return;
            }

            var request = new
            {
                GameId = gameId,
                ActionType = txtPokerActionType.Text,
                Amount = amount
            };

            string response = DoJson("https://localhost:44335/api/games/apply", JsonConvert.SerializeObject(request));

            try
            {
                litPokerApplyActionResult.Text = JToken.Parse(response).ToString(Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "<br/>");
            }
            catch (JsonReaderException)
            {
                litPokerApplyActionResult.Text = HttpUtility.HtmlEncode(response);
            }
        }

        protected void btnPokerBot_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerBotGameId.Text, out Guid gameId))
            {
                litPokerBotResult.Text = "Invalid game id.";
                return;
            }

            string gameState = DoGet($"https://localhost:44335/api/games/{gameId}");

            if (string.IsNullOrWhiteSpace(gameState))
            {
                litPokerBotResult.Text = "Could not load game state.";
                return;
            }

            try
            {
                BotDecisionResponse botResponse = RequestPokerBot(gameState);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Action: {botResponse.ActionType} (Amount: {botResponse.Amount})");
                sb.AppendLine($"Narration: {botResponse.Description}");

                if (!string.IsNullOrWhiteSpace(botResponse.RawModelResponse))
                {
                    sb.AppendLine();
                    sb.AppendLine("Model response:");
                    sb.AppendLine(botResponse.RawModelResponse);
                }

                litPokerBotResult.Text = HttpUtility.HtmlEncode(sb.ToString());
            }
            catch (Exception ex)
            {
                litPokerBotResult.Text = HttpUtility.HtmlEncode("Error calling PokerBot service: " + ex.Message);
            }
        }

        protected void btnDllHash_Click(object sender, EventArgs e)
        {
            string data = txtDllHashInput.Text ?? "";
            try
            {
                string result = PasswordHandler.HashPassword(data);
                litDllHashResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllHashResult.Text = HttpUtility.HtmlEncode("DLL hashing error: " + ex.ToString());
            }
        }

        protected void btnDllVerify_Click(object sender, EventArgs e)
        {
            string verifyData = txtDllVerifyInput.Text ?? "";
            string hashedData = txtDllHashedInput.Text ?? "";
            try
            {
                bool result = PasswordHandler.VerifyPassword(verifyData, hashedData);
                litDllVerifyResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllVerifyResult.Text = HttpUtility.HtmlEncode("DLL hashing error: " + ex.ToString());
            }
        }

        protected void btnPokerDeckVisualize_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(txtPokerVisualizeGameId.Text, out Guid gameId))
            {
                playerDeckView.Visible = true;
                playerDeckView.ShowErrorMessage("Invalid game id.");
                return;
            }

            string gameState = DoGet($"https://localhost:44335/api/games/{gameId}");

            playerDeckView.Visible = true;

            if (string.IsNullOrWhiteSpace(gameState))
            {
                playerDeckView.ShowErrorMessage("Could not load game state.");
                return;
            }

            playerDeckView.RenderFromJson(gameState);
        }

        protected void btnPokerMoneyVisualize_Click(object sender, EventArgs e)
        {

        }
    }
}