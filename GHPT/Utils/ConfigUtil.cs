using GHPT.Configs;
using GHPT.IO;
using Rhino;

namespace GHPT.Utils
{
	public static class ConfigUtil
	{
		private static List<ModelConfig> ConfigList = new();
		public static IReadOnlyList<ModelConfig> Configs => ConfigList;

		public static bool CheckConfiguration()
		{
			return PersistentSettings.RhinoAppSettings.TryGetChild(nameof(ModelConfig), out var allSettings) &&
				allSettings.ChildKeys.Count > 0;
		}

		public static void LoadConfigs()
		{
			PersistentSettings allSettings = null;
			if (!PersistentSettings.RhinoAppSettings.TryGetChild(nameof(ModelConfig), out allSettings))
			{
				allSettings = PersistentSettings.RhinoAppSettings.AddChild(nameof(ModelConfig));
				allSettings.HiddenFromUserInterface = true;
			}

			ConfigList = new(allSettings.ChildKeys.Count);

			var childKeys = allSettings.ChildKeys;
			foreach (string childKey in allSettings.ChildKeys)
			{
				if (!allSettings.TryGetChild(childKey, out PersistentSettings childSettings))
					continue;

				ModelConfig config = GetConfigFromSettings(childKey, childSettings);
				if (!ConfigList.Contains(config) && config.IsValid())
				{
					ConfigList.Add(config);
					ConfigAdded?.Invoke(null, new ConfigArgs(config));
				}
			}
		}

		private static ModelConfig GetConfigFromSettings(string name, PersistentSettings childSettings)
		{
			childSettings.TryGetEnumValue(nameof(ModelConfig.Icon), out ModelIcon icon);
			childSettings.TryGetString(nameof(ModelConfig.Token), out string token);
			childSettings.TryGetString(nameof(ModelConfig.Model), out string model);
            childSettings.TryGetString(nameof(ModelConfig.Url), out string url);
			ModelConfig config = new ModelConfig(name, icon, token, model, url);
			if (!config.IsValid()) { TryMapConfig(config, model); }
			return config;
		}

        /// <summary>
        /// Given that config class has changed this method attempts to map the new url data to the saved configs by matching `Model` property.
        /// `Url` has been introduced as part of the model config class.
        /// `config` is modified.
        /// Returns true if config is valid.
        /// </summary>
        private static bool TryMapConfig(ModelConfig oldConfig, string model)
        {
            oldConfig.Url = Models.ModelOptions.First(c => c.Key == model).Value.Url;
            return oldConfig.IsValid();
        }

        public static void SaveConfig(ModelConfig config)
		{
			if (!ConfigList.Contains(config))
			{
				ConfigList.Add(config);
				ConfigAdded?.Invoke(null, new ConfigArgs(config));
			}

			PersistentSettings allSettings = null;
			if (!PersistentSettings.RhinoAppSettings.TryGetChild(nameof(ModelConfig), out allSettings))
			{
				allSettings = PersistentSettings.RhinoAppSettings.AddChild(nameof(ModelConfig));
				allSettings.HiddenFromUserInterface = true;
			}

			PersistentSettings configSettings = null;
			if (!allSettings.TryGetChild(config.Name, out configSettings))
			{
				configSettings = allSettings.AddChild(config.Name);
				configSettings.HiddenFromUserInterface = true;
			}

			configSettings.SetString(nameof(ModelConfig.Token), config.Token);
			configSettings.SetString(nameof(ModelConfig.Model), config.Model);
            configSettings.SetString(nameof(ModelConfig.Url), config.Url);
            configSettings.SetEnumValue(nameof(ModelConfig.Icon), config.Icon);
		}

		public static void RemoveConfig(ModelConfig config)
		{
			ConfigList.Remove(config);
			ConfigRemoved?.Invoke(null, new ConfigArgs(config));

			PersistentSettings allSettings = null;
			if (!PersistentSettings.RhinoAppSettings.TryGetChild(nameof(ModelConfig), out allSettings))
			{
				allSettings = PersistentSettings.RhinoAppSettings.AddChild(nameof(ModelConfig));
				allSettings.HiddenFromUserInterface = true;
			}

			allSettings.DeleteChild(config.Name);
		}

		internal static EventHandler<ConfigArgs> ConfigAdded;
		internal static EventHandler<ConfigArgs> ConfigRemoved;

		internal class ConfigArgs : EventArgs
		{
			internal readonly ModelConfig Config;
			internal ConfigArgs(ModelConfig config)
			{
				Config = config;
			}
		}

	}
}
