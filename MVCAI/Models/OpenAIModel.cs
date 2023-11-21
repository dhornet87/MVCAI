using Azure;
using Azure.AI.OpenAI;
using static System.Net.WebRequestMethods;

namespace MVCAI.Models
{
    public class OpenAIModel
    {

        public async Task<string> QueryGPT(string query)
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

            completionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "Du bist ein freundlicher, persönlicher Assistent, welcher Dokumente sortiert. In folgende Hauptkategorien: Rechnungen, Gehaltsabrechnungen, Versicherungen, Sonstige Dokumente. Für jede Hauptkategorie kannst du beliebig weitere Unterkategorien anlegen." +
                "Du bekommst entweder Stichworte zu einem Dokument oder das ganze Dokument als Text und gibst dann als Antwort zuerst die Hauptkategorie und dann die Subkategorie zurück."));
            completionsOptions.Messages.Add(new ChatMessage(ChatRole.User, query));

            var response = await openAIClient.GetChatCompletionsAsync(completionsOptions);
            string completion = response.Value.Choices[0].Message.Content;
            return completion;

        }
    }
}
