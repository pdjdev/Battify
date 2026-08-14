<img src="screenshots/battify_main.png">
<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://github.com/user-attachments/assets/f5855e80-f63f-4460-83d8-fa41d451916b">
  <source media="(prefers-color-scheme: light)" srcset="https://github.com/user-attachments/assets/9a7707da-d0c1-48ba-a372-36d2c66351dd">
  <img alt="Battify Banner Image" src="https://github.com/user-attachments/assets/9a7707da-d0c1-48ba-a372-36d2c66351dd">
</picture>

<div align="center">

# Battify

### A Windows tray battery notifier with sleek popups

[English](README.md) | [한국어](README.ko.md) | [日本語](README.ja.md) | [简体中文](README.zh-CN.md) | [Deutsch](README.de.md)

</div>

---

## Download

### Microsoft Store (Recommended)

<a href="https://apps.microsoft.com/detail/9P4FMBB50JV9?referrer=appbadge&mode=direct" target="_blank">
	<img src="https://get.microsoft.com/images/en-us%20dark.svg" width="200"/>
</a>

### Portable version

See the [Releases page](https://github.com/pdjdev/Battify/releases).

## Features

- Battery percentage icon visible from the taskbar
- Notification popups when the charging status changes
- Light and dark theme styling matching Windows 10 and 11
- HiDPI support

## Requirements

- Windows 10 or later
- .NET Framework 4.8, included with Windows 10 and 11

## Screenshots

<img src="screenshots/preview_video.webp" width="800" height="450" />

<img width="800" height="450" alt="2" src="https://github.com/user-attachments/assets/62e1c43c-835c-4c94-b511-d5ad7d16eafa" />

<img width="800" height="450" alt="3" src="https://github.com/user-attachments/assets/4e256541-161c-4dc1-a062-6f7aa51c89ba" />

## Localization

Battify currently provides English, Korean, Japanese, Simplified Chinese, and German translations. The application selects a language based on the Windows display language and falls back to English when a translation is unavailable.

Translation resources are embedded in `Battify.exe`. To add another language, copy `Strings.en.resx` to `Strings.<language-code>.resx` and translate its values.

## License

Made by PBJSoftware (박동준)

Battify is available under the MIT License.
