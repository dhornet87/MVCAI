using Azure;
using Azure.AI.OpenAI;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MVCAI.Models
{
    public class OpenAIModel
    {

        public static async Task<DocumentViewModel> QueryGPT(string query, string kategorie)
        {

        string baseproxyUrl = "https://aoai.hacktogether.net";
        string key = "d2bbbc89-745c-48ce-8a64-3126aeb4b5a1";

        // the full url is appended by /v1/api
        Uri proxyUrl = new(baseproxyUrl + "/v1/api");

        // the full key is appended by "/YOUR-GITHUB-ALIAS"
        AzureKeyCredential token = new(key + "/dhornet87");

        // instantiate the client with the "full" values for the url and key/token
        OpenAIClient openAIClient = new(proxyUrl, token);
            ChatCompletionsOptions completionsOptions = new()
            {
                MaxTokens = 2048,
                Temperature = 0.7f,
                NucleusSamplingFactor = 0.95f,
                DeploymentName = "gpt-35-turbo"
            };
            completionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "Du bist ein deutscher, freundlicher, persönlicher Assistent, welcher Dokumente sortiert und analysiert." +
                $"Du bekommst hierbei das ganze Dokument als Text, welches der Kategorie {kategorie} zugehört und gibst dann als Antwort im Json Format zuerst den \"Titel\" und dann die \"Unterkategorie\" zurück. " +
                $"Außerdem gibst du dann die \"Metadaten\" des Dokuments als Array, welcher aus Objekten besteht mit den Attributen \"Name\" und \"Details\" wieder." +
                $"Am Schluss kannst du noch notwendige Tätigkeiten, die sich aus dem Dokument ergeben anfügen. Dies kennzeichnest du mit einem weiteren Array \"ToDos\", welcher aus Objekten besteht mit den Attributen \"Titel\", \"Beschreibung\" und \"Faelligkeit\" als Datum."));
            completionsOptions.Messages.Add(new ChatMessage(ChatRole.User, query));

            var response = await openAIClient.GetChatCompletionsAsync(completionsOptions);
            string completion = response.Value.Choices[0].Message.Content;

            return ParseResponse(completion);

        }

        private static DocumentViewModel ParseResponse(string response)
        {
            DocumentViewModel documentViewModel = new DocumentViewModel();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                documentViewModel =
                         JsonSerializer.Deserialize<DocumentViewModel>(response, options);
            }
            catch (Exception e)
            {
                var exc = e;
                throw;
            }

            return documentViewModel;
        }
    }
}
