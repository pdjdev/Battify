# Localization

All translation resources are embedded in `Battify.exe`.

To add a language, copy `Strings.en.resx` to `Strings.<language-code>.resx` (for example, `Strings.fr.resx`) and translate every value.

The language is automatically included in the language selector and is used when it matches the Windows display language.

Use `Strings.ko.resx` for Korean; every supported language follows the same filename convention.
If Windows uses an unsupported language, Battify falls back to English.

---

# 다국어 지원

모든 번역 리소스는 `Battify.exe`에 내장되어 있습니다.

새로운 언어를 추가하기 위해서는, 기존에 존재하는 언어 파일을 `Strings.<language-code>.resx`로 복사하고 (예: `Strings.fr.resx`) 모든 값을 번역하시면 됩니다.

추가한 언어 파일은 자동으로 언어 선택기에 포함되며, Windows 표시 언어와 일치할 경우 사용됩니다.

한국어는 `Strings.ko.resx`를 사용하며, 지원되는 모든 언어는 동일한 파일명 규칙을 따릅니다.
지원하지 않는 언어의 Windows 환경일 경우, Battify는 영어로 대체됩니다.