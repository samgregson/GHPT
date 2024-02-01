using GHPT.Configs;

namespace GHPT.IO
{
    public struct ModelConfig
    {
        public string Name { get; set; }
        
        public string Url { get; set; }

        public string Token { get; set; }

        public string Model { get; set; }

        public ModelIcon Icon { get; set; }

        public ModelConfig(string name, ModelIcon icon, string token, string model, string url)
        {
            this.Name = name;
            this.Icon = icon;
            this.Token = token;
            this.Model = model;
        }

        internal bool IsValid() =>
            !string.IsNullOrEmpty(Name) &&
            !string.IsNullOrEmpty(Token) &&
            !string.IsNullOrEmpty(Model) &&
            !string.IsNullOrEmpty(Url);

    }
}
