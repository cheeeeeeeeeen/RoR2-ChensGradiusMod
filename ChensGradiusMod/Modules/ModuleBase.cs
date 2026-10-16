using BepInEx.Configuration;
using System.Globalization;

namespace Chen.GradiusMod.Modules
{
    /// <summary>
    /// The base class of the items and artifacts of this mod. It replaces the module system that TILER2 used to provide.
    /// </summary>
    public abstract class ModuleBase
    {
        /// <summary>
        /// Determines if the module is enabled. Disabled modules are not registered to the game.
        /// </summary>
        public bool enabled { get; protected set; } = true;

        /// <summary>
        /// The internal name of the module, which is the class name.
        /// </summary>
        public string name => GetType().Name;

        /// <summary>
        /// The prefix of the config section that is used by the module, e.g. "Items" or "Artifacts".
        /// </summary>
        protected abstract string configCategoryPrefix { get; }

        /// <summary>
        /// The config section used by the module. This follows the same format TILER2 used so old config files are preserved.
        /// </summary>
        protected string configCategory => $"{configCategoryPrefix}.{name}";

        /// <summary>
        /// The config file where the module binds its config options.
        /// </summary>
        protected ConfigFile config { get; private set; }

        internal void BindConfigFile(ConfigFile configFile) => config = configFile;

        /// <summary>
        /// Binds the config options of the module. Always call base.SetupConfig when overriding.
        /// </summary>
        public virtual void SetupConfig()
        {
            enabled = Bind(nameof(enabled), enabled, "If false, this module will not be loaded at all.");
        }

        /// <summary>
        /// Creates and registers the definitions of the module to the game, such as the ItemDef or the ArtifactDef.
        /// </summary>
        public abstract void SetupAttributes();

        /// <summary>
        /// Sets up the behavior of the module such as language tokens and assets.
        /// </summary>
        public virtual void SetupBehavior()
        {
        }

        /// <summary>
        /// Applies the hooks of the module.
        /// </summary>
        public virtual void Install()
        {
        }

        /// <summary>
        /// Removes the hooks of the module.
        /// </summary>
        public virtual void Uninstall()
        {
        }

        /// <summary>
        /// Binds a config option under the module's config section.
        /// </summary>
        /// <typeparam name="T">Type of the config option</typeparam>
        /// <param name="propertyName">Name of the property that holds the value. It is turned to PascalCase for the config key.</param>
        /// <param name="defaultValue">Default value of the config option</param>
        /// <param name="description">Description of the config option</param>
        /// <returns>The value of the config option</returns>
        protected T Bind<T>(string propertyName, T defaultValue, string description)
        {
            return config.Bind(configCategory, ToKey(propertyName), defaultValue, description).Value;
        }

        /// <summary>
        /// Binds a config option under the module's config section, clamped within a range.
        /// </summary>
        /// <typeparam name="T">Type of the config option</typeparam>
        /// <param name="propertyName">Name of the property that holds the value. It is turned to PascalCase for the config key.</param>
        /// <param name="defaultValue">Default value of the config option</param>
        /// <param name="description">Description of the config option</param>
        /// <param name="min">Minimum value of the config option</param>
        /// <param name="max">Maximum value of the config option</param>
        /// <returns>The value of the config option</returns>
        protected T Bind<T>(string propertyName, T defaultValue, string description, T min, T max) where T : System.IComparable
        {
            ConfigDescription configDescription = new ConfigDescription(description, new AcceptableValueRange<T>(min, max));
            return config.Bind(configCategory, ToKey(propertyName), defaultValue, configDescription).Value;
        }

        /// <summary>
        /// Formats a number into a percentage string, e.g. 0.25 becomes "25%".
        /// </summary>
        /// <param name="value">The number to format</param>
        /// <param name="precision">The number of decimal places</param>
        /// <returns>The formatted percentage string</returns>
        public static string Pct(float value, uint precision = 0)
        {
            return (value * 100f).ToString("N" + precision, CultureInfo.InvariantCulture) + "%";
        }

        /// <summary>
        /// Returns the plural suffix "s" when the number is not 1.
        /// </summary>
        /// <param name="number">The number to check</param>
        /// <returns>"s" if plural, empty string if singular</returns>
        public static string NPlur(int number) => number == 1 ? string.Empty : "s";

        private static string ToKey(string propertyName) => char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1);
    }
}
