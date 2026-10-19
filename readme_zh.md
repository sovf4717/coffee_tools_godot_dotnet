# ☕ coffee_tools — 中文说明

> 📄 本页为**中文说明**。英文版 → [README.md](README.md)

**coffee_tools 是一个半成品的咖啡浓度计算日志项目（Work in Progress）。**

只要手边有一台**能测遮光率的仪器**（例如廉价折射计 + 手机镜头/传感器方案），把测得的**遮光率（透光率/吸光率）数值**输入本项目，经过内置计算后就能得出**咖啡浓度（TDS 近似值）**，并可以生成、保存和查看自己的冲煮日志。

> 计算所用的数据与标定方法来源于：[Calibrating a Cheap Refractometer — Quantitative Coffee](https://quantitativecafe.com/2022/06/16/calibrating-a-cheap-refractometer/)（仓库内也附带了该网页的快捷方式文件 `校准廉价折射计.url`）。

### 主要功能（现状）

- 输入遮光率 → 计算咖啡浓度
- 日志模板：创建咖啡冲煮记录（`日志模板.tscn`）
- 保存 / 加载日志数据（JSON 持久化）
- 键盘操作与界面定位（`KeyboardHandler.cs`）
- 已导出 Android 安装包，可在 **Releases** 中下载 `coffee_tools.apk`

### 技术栈

- [Godot 4.5.1](https://godotengine.org/)（.NET / C# Mono 后端）
- C# 脚本：`Calculator.cs`、`LoadData.cs`、`SaveData.cs`、`KeyboardHandler.cs` 等

### 获取应用

- 直接下载：仓库 **Releases** 页面 → `coffee_tools.apk`（Android，需允许安装未知来源应用）
- 从源码构建：安装 [Godot 4.5.1 (.NET 版)](https://godotengine.org/download) 后打开本项目，导出 Android 目标即可

> 注意：项目目前为**半成品**，功能与界面仍在持续完善中，欢迎提交 Issue / PR。

---

📄 英文版 → [README.md](README.md)