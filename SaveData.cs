using Godot;
using System;
using System.IO;
using System.Text.Json;

public partial class SaveData : Button
{
	// 引用需保存的节点（全部替换为TextEdit）
	private TextEdit 折光率输入;       // 输入节点 → TextEdit
	private Label 浓度显示;             // 显示节点 → Label（保持Label不变）
	private TextEdit 粉重输入;         // 输入节点 → TextEdit
	private TextEdit 液重输入;         // 输入节点 → TextEdit
	private Label 萃取率显示;           // 显示节点 → Label（保持Label不变）
	private TextEdit 温度输入;         // 输入节点 → TextEdit
	private TextEdit 研磨度输入;       // 输入节点 → TextEdit
	private TextEdit 时间输入;         // 输入节点 → TextEdit
	private TextEdit 流速输入;         // 输入节点 → TextEdit
	private TextEdit 制作方式输入;     // 输入节点 → TextEdit
	private TextEdit 豆子输入;         // 输入节点 → TextEdit
	private TextEdit 下次制作建议;     // 输入节点 → TextEdit（原本已是TextEdit，保持一致）
	private TextEdit 备注;             // 多行输入 → TextEdit（保持一致）

	// 定义目标保存路径（安卓外部存储目录）
	private const string TargetDir = "/storage/emulated/0/Android/data/com.example.coffee_tools/coffee_data/";

	public override void _Ready()
	{
		// 获取节点引用（GetNode类型改为TextEdit）
		折光率输入 = GetNode<TextEdit>("../折光率输入");
		浓度显示 = GetNode<Label>("../浓度显示");
		粉重输入 = GetNode<TextEdit>("../粉重输入");
		液重输入 = GetNode<TextEdit>("../液重输入");
		萃取率显示 = GetNode<Label>("../萃取率显示");
		温度输入 = GetNode<TextEdit>("../温度输入");
		研磨度输入 = GetNode<TextEdit>("../研磨度输入");
		时间输入 = GetNode<TextEdit>("../时间输入");
		流速输入 = GetNode<TextEdit>("../流速输入");
		制作方式输入 = GetNode<TextEdit>("../制作方式输入");
		豆子输入 = GetNode<TextEdit>("../豆子输入");
		下次制作建议 = GetNode<TextEdit>("../下次制作建议");
		备注 = GetNode<TextEdit>("../备注");

		// 绑定按钮点击事件
		Pressed += OnSavePressed;
	}

	private void OnSavePressed()
	{
		// 封装数据（TextEdit的Text属性用法与LineEdit一致，无需修改）
		var data = new
		{
			折光率 = 折光率输入.Text,
			浓度 = 浓度显示.Text,
			粉重 = 粉重输入.Text,
			液重 = 液重输入.Text,
			萃取率 = 萃取率显示.Text,
			温度 = 温度输入.Text,
			研磨度 = 研磨度输入.Text,
			时间 = 时间输入.Text,
			流速 = 流速输入.Text,
			制作方式 = 制作方式输入.Text,
			豆子输入 = 豆子输入.Text,
			下次制作建议 = 下次制作建议.Text,
			备注 = 备注.Text,
			日期时间 = DateTime.Now.ToString("yyyyMMdd_HHmmss")
		};

		try
		{
			// 序列化JSON
			string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

			// 检查目录是否存在，不存在则创建
			if (!Directory.Exists(TargetDir))
			{
				Directory.CreateDirectory(TargetDir);
				GD.Print("目录不存在，已创建：" + TargetDir);
			}

			// 拼接文件路径并适配安卓分隔符
			string filePath = Path.Combine(TargetDir, $"{data.日期时间}.json").Replace("\\", "/");

			// 写入文件
			File.WriteAllText(filePath, json);

			GD.Print("数据保存成功：" + filePath);
		}
		catch (Exception e)
		{
			GD.PrintErr($"保存失败：{e.Message}");
			GD.PrintErr($"堆栈信息：{e.StackTrace}");
		}
	}
}
