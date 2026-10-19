using Godot;
using System;

public partial class Calculator : Control
{
	private TextEdit 折光率输入;  
	private TextEdit 粉重输入;    
	private TextEdit 液重输入;    
	private Label 浓度显示;       
	private Label 萃取率显示;     

	public override void _Ready()
	{
		// 修正节点路径：输入框和显示标签在 "Control" 子节点下
		折光率输入 = GetNode<TextEdit>("Control/折光率输入");
		粉重输入 = GetNode<TextEdit>("Control/粉重输入");
		液重输入 = GetNode<TextEdit>("Control/液重输入");
		浓度显示 = GetNode<Label>("Control/浓度显示");
		萃取率显示 = GetNode<Label>("Control/萃取率显示");

		// 容错处理：检查节点是否存在
		if (折光率输入 == null) GD.PrintErr("未找到节点：Control/折光率输入");
		if (粉重输入 == null) GD.PrintErr("未找到节点：Control/粉重输入");
		if (液重输入 == null) GD.PrintErr("未找到节点：Control/液重输入");
		if (浓度显示 == null) GD.PrintErr("未找到节点：Control/浓度显示");
		if (萃取率显示 == null) GD.PrintErr("未找到节点：Control/萃取率显示");

		// 绑定输入框文本变化事件
		if (折光率输入 != null)
			折光率输入.TextChanged += OnInputValuesChanged;
		if (粉重输入 != null)
			粉重输入.TextChanged += OnInputValuesChanged;
		if (液重输入 != null)
			液重输入.TextChanged += OnInputValuesChanged;
	}

	private void OnInputValuesChanged()
	{
		// 先检查必要节点是否存在
		if (折光率输入 == null || 浓度显示 == null || 萃取率显示 == null)
			return;

		// 计算浓度
		if (double.TryParse(折光率输入.Text, out double 折光率))
		{
			double 项1 = -280.2 * Math.Pow(折光率 - 1.3330, 2);
			double 项2 = 566.7 * (折光率 - 1.3330);
			double 浓度值 = 项1 + 项2 + 0.0059;
			浓度显示.Text = 浓度值.ToString("F4") + "%";
		}
		else
		{
			浓度显示.Text = "请输入有效数字";
			萃取率显示.Text = "";
			return;
		}

		// 计算萃取率
		string 浓度文本 = 浓度显示.Text.Replace("%", "");
		if (double.TryParse(浓度文本, out double 浓度) &&
			液重输入 != null && double.TryParse(液重输入.Text, out double 液重) &&
			粉重输入 != null && double.TryParse(粉重输入.Text, out double 粉重) &&
			粉重 > 0)
		{
			double 萃取率 = 浓度 * 液重 / 粉重;
			萃取率显示.Text = $"{萃取率:F2}%";
		}
		else
		{
			萃取率显示.Text = "请输入有效参数";
		}
	}
}
