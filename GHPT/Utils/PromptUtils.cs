using GHPT.IO;
using GHPT.Prompts;
using System.Threading.Tasks;

namespace GHPT.Utils
{

	public static class PromptUtils
	{

		private static readonly string[] SPLITTER = { "```json", "```" };

		public static string GetChatGPTJson(string chatGPTResponse)
		{
			string[] jsons = chatGPTResponse.Split( SPLITTER,
										System.StringSplitOptions.RemoveEmptyEntries);

			string latestJson = jsons.Last();

			return latestJson;
		}

		public static PromptData GetPromptDataFromResponse(string chatGPTJson)
		{
			if (chatGPTJson.ToLowerInvariant().Contains(Prompt.TOO_COMPLEX))
			{
				return new PromptData()
				{
					Additions = new List<Addition>(),
					Connections = new List<ConnectionPairing>(),
					Advice = Prompt.TOO_COMPLEX
				};
			}
            
            try
			{
				PromptData result = Newtonsoft.Json.JsonConvert.DeserializeObject<PromptData>(chatGPTJson);
				result.ComputeTiers();
				return result;
			}
			catch (Exception ex)
			{
				return new PromptData()
				{
					Additions = new List<Addition>(),
					Connections = new List<ConnectionPairing>(),
					Advice = "Exception: " + ex.Message
				};
			}
		}

		public static async Task<PromptData> AskQuestion(ModelConfig config, string question, double temperature)
		{
			try
			{
				string prompt = Prompt.GetPrompt(question);
				var payload = await ClientUtil.Ask(config, prompt, temperature);
				string payloadJson = payload.Choices.FirstOrDefault().Message.Content;

				if (payloadJson.ToLowerInvariant().Contains(Prompt.TOO_COMPLEX.ToLowerInvariant()))
				{
					return new PromptData()
					{
						Advice = Prompt.TOO_COMPLEX
					};
				}

				var json = GetChatGPTJson(payloadJson);
                var returnValue = GetPromptDataFromResponse(json);

				return returnValue;
			}
			catch (Exception ex)
			{
				return new PromptData()
				{
					Advice = ex.Message
				};
			}
		}

	}

}
