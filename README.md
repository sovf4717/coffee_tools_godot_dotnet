# ☕ coffee_tools

> 📄 This README is in **English**. 中文说明 → [readme_zh.md](readme_zh.md)

**coffee_tools is a work-in-progress (semi-finished) coffee concentration logging app.**

All you need is any instrument that can measure **light transmission (obscuration/absorbance)** — for example a cheap refractometer paired with a phone-based setup. Enter the measured value into this app, and it will compute the **coffee concentration (approx. TDS)** for you, so you can build, save, and review your own brewing logs.

> The data and calibration method behind the calculation come from: [Calibrating a Cheap Refractometer — Quantitative Coffee](https://quantitativecafe.com/2022/06/16/calibrating-a-cheap-refractometer/) (a Windows shortcut to that page is also included in this repo: `校准廉价折射计.url`).

### Features (as-is)

- Enter a light-transmission reading → get coffee concentration
- Brew log template (`日志模板.tscn`)
- Save / load log data (JSON persistence)
- Keyboard handling & UI positioning (`KeyboardHandler.cs`)
- Android build available — download `coffee_tools.apk` from **Releases**

### Tech

- [Godot 4.5.1](https://godotengine.org/) (.NET / C# Mono backend)
- C# scripts: `Calculator.cs`, `LoadData.cs`, `SaveData.cs`, `KeyboardHandler.cs`, etc.

### Get the app

- Download the `coffee_tools.apk` from the repo **Releases** page (Android; you may need to allow "install unknown apps")
- Or build from source: install [Godot 4.5.1 (.NET)](https://godotengine.org/download) and export for Android

> Note: this project is a **work in progress** — features and UI are still being polished. Issues and PRs are welcome.

---

📄 中文说明 → [readme_zh.md](readme_zh.md)
