namespace GHPT.Configs
{
    public class ModelProperties
    {
        public ModelIcon Icon { get; }
        public string Url { get; }

        public ModelProperties(ModelIcon icon, string url)
        {
            Icon = icon;
            Url = url;
        }
    }
}
