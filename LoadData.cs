// 引入Godot引擎核心命名空间，提供所有Godot节点和API的基础支持
using Godot;
// 引入系统核心命名空间，提供异常处理、基础类型等核心功能
using System;
// 引入文件操作命名空间，用于目录/文件的读取、检查等操作
using System.IO;
// 引入JSON序列化/反序列化命名空间，用于解析咖啡记录的JSON文件
using System.Text.Json;

// 定义LoadData类，继承自Control（Godot的基础控件节点），用于加载咖啡记录数据并渲染UI
public partial class LoadData : Control
{
	// 常量：咖啡数据文件的存储目录（Android设备的外部存储路径）
	// 路径说明：Android应用私有存储目录，避免权限问题
	private const string 数据目录 = "/storage/emulated/0/Android/data/com.example.coffee_tools/coffee_data/";
	
	// 导出字段：日志模板场景（可在Godot编辑器中指定，用于生成每条记录的UI实例）
	[Export] private PackedScene 日志模板;
	// 成员变量：用于承载所有日志实例的垂直容器节点（VBoxContainer）
	private VBoxContainer 实例容器;

	// 重写Godot节点的_Ready方法（节点进入场景树后执行，初始化逻辑入口）
	public override void _Ready()
	{
		// 获取实例容器节点（使用GetNodeOrNull容错，避免路径错误导致崩溃）
		// 路径说明：上级节点 -> center容器 -> 标签容器 -> 日志标签页 -> 滚动容器 -> 垂直容器
		实例容器 = GetNodeOrNull<VBoxContainer>("../center/TabContainer/日志/ScrollContainer/VBoxContainer");
		// 容错处理：若容器节点未找到，打印错误日志并终止初始化
		if (实例容器 == null)
		{
			GD.PrintErr("错误：未找到VBoxContainer容器，请检查路径！");
			return;
		}

		// 设置容器布局属性：强制垂直方向扩展填充父节点空间
		实例容器.SizeFlagsVertical = SizeFlags.ExpandFill;
		// 重置容器最小尺寸，避免布局挤压
		实例容器.CustomMinimumSize = new Vector2(0, 0);

		// 容错加载日志模板：若编辑器未指定，尝试从默认路径加载
		if (日志模板 == null)
		{
			// 从资源路径加载日志模板场景（res://为Godot项目资源根路径）
			日志模板 = ResourceLoader.Load<PackedScene>("res://日志模板.tscn");
			// 二次容错：若模板仍未加载成功，打印错误日志并终止
			if (日志模板 == null)
			{
				GD.PrintErr("错误：日志模板加载失败，请确认路径res://日志模板.tscn存在！");
				return;
			}
		}

		// 核心逻辑：加载所有咖啡日志文件并生成UI实例
		LoadAllCoffeeLogs();
	}

	/// <summary>
	/// 核心方法：加载数据目录中所有JSON格式的咖啡记录，并生成对应的UI实例
	/// 包含完整的异常处理和容错逻辑，避免单个文件错误导致整体加载失败
	/// </summary>
	private void LoadAllCoffeeLogs()
	{
		// 第一步：清空容器中已有的旧实例（避免重复加载）
		foreach (Node child in 实例容器.GetChildren())
		{
			// 从容器中移除子节点
			实例容器.RemoveChild(child);
			// 标记节点待销毁（Godot安全销毁方式，避免内存泄漏）
			child.QueueFree();
		}

		// 第二步：检查数据目录是否存在（首次使用时目录可能未创建）
		if (!Directory.Exists(数据目录))
		{
			// 打印提示日志，不终止流程（目录不存在时无数据可加载）
			GD.Print($"提示：数据目录不存在，路径：{数据目录}");
			return;
		}

		// 第三步：获取目录中所有.json后缀的文件（咖啡记录文件）
		string[] 所有JSON文件 = Directory.GetFiles(数据目录, "*.json");
		// 打印日志：告知找到的记录数量，便于调试
		GD.Print($"提示：共发现 {所有JSON文件.Length} 条咖啡记录");

		// 第四步：遍历所有JSON文件，逐个处理（单文件异常不影响其他文件）
		foreach (string 文件路径 in 所有JSON文件)
		{
			// 异常捕获：包裹单个文件的处理逻辑，避免单个文件错误中断循环
			try
			{
				// 子容错1：检查文件是否存在（防止文件被删除/移动导致的异常）
				if (!File.Exists(文件路径))
				{
					GD.PrintErr($"文件不存在：{文件路径}，已跳过");
					// 跳过当前文件，继续处理下一个
					continue;
				}

				// 读取文件内容：一次性读取整个JSON文件的文本内容
				string JSON内容 = File.ReadAllText(文件路径);
				// 子容错2：检查文件内容是否为空（避免空文件解析异常）
				if (string.IsNullOrEmpty(JSON内容))
				{
					GD.PrintErr($"文件内容为空：{文件路径}，已跳过");
					continue;
				}
				
				// 反序列化：将JSON文本转换为CoffeeLogData数据对象
				// 自动匹配字段名（需保证JSON键名与类属性名一致）
				CoffeeLogData 记录数据 = JsonSerializer.Deserialize<CoffeeLogData>(JSON内容);
				// 子容错3：检查反序列化结果是否为空（JSON格式错误时返回null）
				if (记录数据 == null)
				{
					GD.PrintErr($"文件解析为空：{文件路径}，已跳过");
					continue;
				}

				// 实例化模板：从日志模板场景创建Panel节点实例（单条记录的UI载体）
				Panel 记录实例 = 日志模板.Instantiate<Panel>();
				// 子容错4：检查模板实例化是否成功（模板损坏时返回null）
				if (记录实例 == null)
				{
					GD.PrintErr($"模板实例化失败，文件：{文件路径}，已跳过");
					continue;
				}

				// 设置实例属性：确保UI实例可见
				记录实例.Visible = true;
				// 强制设置实例尺寸（统一UI样式，避免模板尺寸不一致）
				记录实例.Size = new Vector2(540.0f, 112.0f);

				// 将实例添加到容器中（渲染到界面）
				实例容器.AddChild(记录实例);
				// 填充数据：将解析后的咖啡记录数据填入UI实例的Label节点
				FillInstanceData(记录实例, 记录数据);
				// 打印成功日志：便于追踪加载状态
				GD.Print($"成功加载：{文件路径}");
			}
			catch (Exception 异常)
			{
				// 捕获所有未预期的异常，打印错误信息并跳过当前文件
				GD.PrintErr($"处理文件 {文件路径} 出错：{异常.Message}，已跳过");
			}
		}
	}

	/// <summary>
	/// 辅助方法：将咖啡记录数据填充到单个UI实例的对应Label节点中
	/// 封装Label节点的查找和文本设置逻辑，统一容错处理
	/// </summary>
	/// <param name="实例">单个日志的Panel实例</param>
	/// <param name="数据">解析后的咖啡记录数据对象</param>
	private void FillInstanceData(Panel 实例, CoffeeLogData 数据)
	{
		// 依次设置每个字段的文本（调用封装的SetLabelText方法，带容错）
		SetLabelText(实例, "VBoxContainer/日期时间", 数据.日期时间);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/折光率", 数据.折光率);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/浓度", 数据.浓度);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/粉重", 数据.粉重);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/液重", 数据.液重);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/萃取率", 数据.萃取率);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/温度", 数据.温度);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/研磨度", 数据.研磨度);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/时间", 数据.时间);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/流速", 数据.流速);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/制作方式", 数据.制作方式);
		SetLabelText(实例, "VBoxContainer/HBoxContainer/豆子", 数据.豆子输入);
		SetLabelText(实例, "VBoxContainer/下次制作建议", 数据.下次制作建议);
		SetLabelText(实例, "VBoxContainer/备注", 数据.备注);
	}

	/// <summary>
	/// 工具方法：安全设置Label节点的文本内容
	/// 封装节点查找和文本赋值逻辑，避免空引用异常
	/// </summary>
	/// <param name="父节点">查找的根节点（单条日志的Panel实例）</param>
	/// <param name="节点路径">Label节点相对于父节点的路径</param>
	/// <param name="文本内容">要设置的文本（可为null）</param>
	private void SetLabelText(Control 父节点, string 节点路径, string 文本内容)
	{
		// 容错查找Label节点（避免路径错误导致崩溃）
		Label 目标标签 = 父节点.GetNodeOrNull<Label>(节点路径);
		// 若节点找到，设置文本（null时显示"无数据"）
		if (目标标签 != null)
		{
			目标标签.Text = 文本内容 ?? "无数据";
		}
		else
		{
			// 节点未找到时打印警告日志，便于调试UI结构问题
			GD.PrintErr($"警告：模板中未找到节点，路径：{节点路径}");
		}
	}

	/// <summary>
	/// 内部数据模型：咖啡记录的JSON结构映射类
	/// 字段名需与JSON文件中的键名完全一致（大小写敏感）
	/// 所有字段为string类型，适配前端输入的多样化格式
	/// </summary>
	private class CoffeeLogData
	{
		public string 折光率 { get; set; }          // 咖啡液折光率（Brix值）
		public string 浓度 { get; set; }            // 咖啡浓度（%）
		public string 粉重 { get; set; }            // 咖啡粉重量（g）
		public string 液重 { get; set; }            // 萃取液重量（g）
		public string 萃取率 { get; set; }          // 萃取率（%）
		public string 温度 { get; set; }            // 萃取水温（℃）
		public string 研磨度 { get; set; }          // 咖啡豆研磨度（刻度/数值）
		public string 时间 { get; set; }            // 萃取时间（s）
		public string 流速 { get; set; }            // 萃取流速（g/s）
		public string 制作方式 { get; set; }        // 制作方式（如手冲、意式、冷萃）
		public string 豆子输入 { get; set; }        // 咖啡豆信息（品种、烘焙度、产地）
		public string 下次制作建议 { get; set; }    // 下次调整建议（主观记录）
		public string 备注 { get; set; }            // 额外备注（口感、环境等）
		public string 日期时间 { get; set; }        // 记录创建的日期时间（文件名对应）
	}
}
