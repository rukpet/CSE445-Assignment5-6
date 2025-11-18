using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PokerBot
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PokerBotService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PokerBotService.svc or PokerBotService.svc.cs at the Solution Explorer and start debugging.
    public class PokerBotService : IPokerBotService
    {
        public void DoWork()
        {
            string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
            string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

            // For older Windows when targeting .NET Framework:
            // System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            // Build the same JSON body as your curl (contents + generationConfig + responseSchema).
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = "List a few popular cookie recipes, and include the amounts of ingredients." }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    responseSchema = new
                    {
                        type = "ARRAY",
                        items = new
                        {
                            type = "OBJECT",
                            properties = new
                            {
                                recipeName = new { type = "STRING" },
                                ingredients = new
                                {
                                    type = "ARRAY",
                                    items = new { type = "STRING" }
                                }
                            },
                            propertyOrdering = new[] { "recipeName", "ingredients" }
                        }
                    }
                }
            };

            string jsonBody = JsonConvert.SerializeObject(payload);

            using (HttpClient http = new HttpClient())
            using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url))
            {
                req.Headers.Add("x-goog-api-key", apiKey);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using (HttpResponseMessage res = await http.SendAsync(req).ConfigureAwait(false))
                {
                    string responseText = await res.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!res.IsSuccessStatusCode)
                    {
                        Console.Error.WriteLine("Request failed: " + res.StatusCode);
                        Console.Error.WriteLine(responseText);
                        return null;
                    }

                    return responseText;
                }
            }
        }
    }
}
