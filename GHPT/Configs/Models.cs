namespace GHPT.Configs
{
    public static class Models
    {
        const string openAiUrl = "https://api.openai.com/v1/chat/completions";
        const string togetherAiUrl = "https://api.together.xyz/v1/chat/completions";

        public static Dictionary<string, ModelProperties> ModelOptions = new()
        {
            { "gpt-4", new ModelProperties(ModelIcon.GPT4, openAiUrl)},
            { "gpt-3.5-turbo",  new ModelProperties(ModelIcon.GPT3_5, openAiUrl) },
            { "gpt-3.5-turbo-0301", new ModelProperties(ModelIcon.GPT3_5, openAiUrl) },
            { "gpt-3.5-turbo-0613",  new ModelProperties(ModelIcon.GPT3_5, openAiUrl) },
            { "gpt-3.5-turbo-16k",  new ModelProperties(ModelIcon.GPT3_5, openAiUrl) },
            { "gpt-3.5-turbo-16k-0613", new ModelProperties(ModelIcon.GPT3_5, openAiUrl) },
            { "mistralai/Mixtral-8x7B-Instruct-v0.1", new ModelProperties(ModelIcon.None, togetherAiUrl) },
            { "togethercomputer/llama-2-70b-chat", new ModelProperties(ModelIcon.None, togetherAiUrl) },
            { "togethercomputer/CodeLlama-34b-Instruct", new ModelProperties(ModelIcon.None, togetherAiUrl) }
        };
    }
}
