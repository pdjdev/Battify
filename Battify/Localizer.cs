using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Markup;

namespace Battify
{
    internal static class Localizer
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("Battify.Strings", typeof(Localizer).Assembly);
        private static readonly ResourceManager EnglishResourceManager = new ResourceManager("Battify.Strings.en", typeof(Localizer).Assembly);

        public static string Get(string key)
        {
            var resourceManager = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en"
                ? EnglishResourceManager
                : ResourceManager;

            // The English resource is embedded in the main assembly, so it is
            // intentionally read as the invariant resource set rather than as
            // a satellite assembly.
            var culture = ReferenceEquals(resourceManager, EnglishResourceManager)
                ? CultureInfo.InvariantCulture
                : CultureInfo.CurrentUICulture;

            return resourceManager.GetString(key, culture) ?? key;
        }

        public static string Format(string key, params object[] arguments) =>
            string.Format(CultureInfo.CurrentCulture, Get(key), arguments);

        public static void ApplyConfiguredCulture()
        {
            var language = Settings.Default.language;
            if (language == "system")
            {
                // Korean is the only non-English translation currently
                // available; all other Windows UI languages use English.
                language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ko"
                    ? "ko"
                    : "en";
            }

            if (language == "ko" || language == "en")
            {
                var culture = CultureInfo.GetCultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }
    }

    [MarkupExtensionReturnType(typeof(string))]
    public sealed class LocExtension : MarkupExtension
    {
        public LocExtension(string key) => Key = key;
        public string Key { get; }

        public override object ProvideValue(IServiceProvider serviceProvider) => Localizer.Get(Key);
    }
}
