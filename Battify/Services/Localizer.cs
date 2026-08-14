using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Data;
using System.Windows.Markup;

namespace Battify
{
    internal static class Localizer
    {
        private const string ResourceBaseName = "Battify.Localization.Strings";
        private static readonly Assembly Assembly = typeof(Localizer).Assembly;
        private static readonly ResourceManager EnglishResourceManager = new ResourceManager($"{ResourceBaseName}.en", Assembly);
        private static readonly HashSet<string> EmbeddedLanguages = Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith($"{ResourceBaseName}.", StringComparison.Ordinal) && name.EndsWith(".resources", StringComparison.Ordinal))
            .Select(name => name.Substring(ResourceBaseName.Length + 1, name.Length - ResourceBaseName.Length - ".resources".Length - 1))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, ResourceManager> TranslationResourceManagers = new(StringComparer.OrdinalIgnoreCase);

        public static event EventHandler? CultureChanged;

        public static string Get(string key)
        {
            var language = ResolveLanguage(CultureInfo.CurrentUICulture);
            var resourceManager = GetTranslationResourceManager(language);

            // Translations are embedded in the main assembly, so they are read
            // as invariant resource sets rather than as satellite assemblies.
            return resourceManager.GetString(key, CultureInfo.InvariantCulture) ?? key;
        }

        public static string Format(string key, params object[] arguments) =>
            string.Format(CultureInfo.CurrentCulture, Get(key), arguments);

        public static IEnumerable<string> GetAvailableLanguages() =>
            EmbeddedLanguages.OrderBy(language => language);

        public static void ApplyConfiguredCulture()
        {
            ApplyCulture(Settings.Default.language);
        }

        public static void ApplyCulture(string language)
        {
            if (language == "system")
            {
                var systemLanguage = ResolveLanguage(CultureInfo.InstalledUICulture);
                language = EmbeddedLanguages.Contains(systemLanguage)
                    ? systemLanguage
                    : "en";
            }

            if (EmbeddedLanguages.Contains(language))
            {
                var culture = CultureInfo.GetCultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        private static ResourceManager GetTranslationResourceManager(string language)
        {
            if (language == "en" || !EmbeddedLanguages.Contains(language))
                return EnglishResourceManager;

            if (!TranslationResourceManagers.TryGetValue(language, out var resourceManager))
            {
                resourceManager = new ResourceManager($"{ResourceBaseName}.{language}", Assembly);
                TranslationResourceManagers.Add(language, resourceManager);
            }

            return resourceManager;
        }

        private static string ResolveLanguage(CultureInfo culture)
        {
            // Chinese needs a script-specific resource: "zh" alone does not
            // distinguish Simplified Chinese from Traditional Chinese.
            if (culture.Name.StartsWith("zh-Hans", StringComparison.OrdinalIgnoreCase)
                || culture.Name.Equals("zh-CN", StringComparison.OrdinalIgnoreCase)
                || culture.Name.Equals("zh-SG", StringComparison.OrdinalIgnoreCase))
            {
                return "zh-Hans";
            }

            return culture.TwoLetterISOLanguageName;
        }
    }

    [MarkupExtensionReturnType(typeof(string))]
    public sealed class LocExtension : MarkupExtension
    {
        public LocExtension(string key) => Key = key;
        public string Key { get; }

        public override object ProvideValue(IServiceProvider serviceProvider) =>
            new System.Windows.Data.Binding($"[{Key}]")
            {
                Source = LocalizationBindingSource.Instance,
                Mode = BindingMode.OneWay
            }.ProvideValue(serviceProvider);
    }

    public sealed class LocalizationBindingSource : INotifyPropertyChanged
    {
        public static LocalizationBindingSource Instance { get; } = new();

        private LocalizationBindingSource()
        {
            Localizer.CultureChanged += (_, _) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

        public string this[string key] => Localizer.Get(key);

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
