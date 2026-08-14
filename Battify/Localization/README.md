# Localization

All translation resources are embedded in `Battify.exe`.

To add a language, copy `Strings.en.resx` to `Strings.<language-code>.resx`
(for example, `Strings.fr.resx`) and translate every value. The language is
automatically included in the language selector and is used when it matches
the Windows display language.

Use `Strings.ko.resx` for Korean; every supported language follows the same
filename convention. If Windows uses an unsupported language, Battify falls
back to English.
