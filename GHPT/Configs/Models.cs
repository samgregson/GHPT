namespace GHPT.Configs
{
    public static class Models
    {
        public static Dictionary<string, GPTVersion> ModelOptions = new()
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
