using Godot;
using System;

/// <summary>
/// 挂载到路径为“Control/输出节点树”的节点上（父节点为Control）
/// 适配 Godot 4.5.1 + .NET 8.0，支持Control节点的交互与布局特性
/// </summary>
public partial class NodeTreePrinter : Control // 继承自Control而非Node，适配父节点特性
{
	// 可选：是否输出到日志文件（默认仅输出到控制台）
	[Export] private bool _exportToFile = false;
	// 可选：日志文件保存路径（项目根目录下）
	[Export] private string _logFilePath = "node_tree_log.txt";
	// 可选：是否包含节点类型信息
	[Export] private bool _showNodeType = true;

	// 节点树输出的入口方法（可通过按钮触发或自动执行）
	[Export] private bool _autoPrintOnReady = true;

	// 可选：绑定UI按钮触发输出（适配Control节点的交互）
	[Export] private Button _printButton;

	public override void _Ready()
	{
		// 自动执行节点树输出
		if (_autoPrintOnReady)
		{
			PrintNodeTree();
		}

		// 绑定按钮点击事件（适配Control节点的UI交互）
		if (_printButton != null)
		{
			_printButton.Pressed += OnPrintButtonPressed;
		}
	}

	/// <summary>
	/// 输出节点树结构（默认从场景根节点开始遍历，也可指定起始节点）
	/// </summary>
	/// <param name="startNode">起始遍历节点（默认传null则从场景根节点开始）</param>
	public void PrintNodeTree(Node startNode = null)
	{
		// 若未指定起始节点，则从场景根节点开始遍历（也可改为从"Control"父节点开始）
		Node rootNode = startNode ?? GetTree().Root.GetChild(0);
		// 若需从"Control"父节点开始遍历，取消下面注释：
		// Node rootNode = startNode ?? GetParent(); // GetParent()即当前节点的父节点"Control"

		GD.Print("=====================================");
		GD.Print($"节点树结构（起始节点：{rootNode.Name}，路径：{rootNode.GetPath()}）");
		GD.Print("=====================================");

		// 递归遍历节点
		string treeContent = TraverseNodeTree(rootNode, 0);
		
		// 输出到控制台
		GD.Print(treeContent);
		
		// 可选：输出到日志文件
		if (_exportToFile)
		{
			SaveTreeToFile(treeContent);
			GD.Print($"节点树已保存到文件：{ProjectSettings.GlobalizePath(_logFilePath)}");
		}

		GD.Print("=====================================\n");
	}

	/// <summary>
	/// 递归遍历节点树，生成带缩进的节点结构字符串
	/// </summary>
	/// <param name="currentNode">当前遍历的节点</param>
	/// <param name="depth">当前节点层级（用于缩进）</param>
	/// <returns>节点树结构字符串</returns>
	private string TraverseNodeTree(Node currentNode, int depth)
	{
		if (currentNode == null) return "";

		// 生成缩进（每层增加 4 个空格）
		string indent = new string(' ', depth * 4);
		
		// 节点信息：名称 + 类型 + 路径（可选）
		string nodePath = currentNode.GetPath();
		string nodeInfo = _showNodeType 
			? $"{indent}[{currentNode.GetType().Name}] {currentNode.Name}（路径：{nodePath}）" 
			: $"{indent}{currentNode.Name}（路径：{nodePath}）";

		// 递归遍历子节点
		foreach (Node child in currentNode.GetChildren())
		{
			nodeInfo += "\n" + TraverseNodeTree(child, depth + 1);
		}

		return nodeInfo;
	}

	/// <summary>
	/// 将节点树保存到文件（适配Godot 4.5.1的FileAccess.ModeFlags枚举）
	/// </summary>
	/// <param name="content">节点树内容</param>
	private void SaveTreeToFile(string content)
	{
		try
		{
			string fullPath = ProjectSettings.GlobalizePath(_logFilePath);
			using var file = FileAccess.Open(fullPath, FileAccess.ModeFlags.Write);
			
			if (file == null)
			{
				GD.PrintErr($"无法打开文件：{fullPath}");
				return;
			}
			
			file.StoreString($"节点树生成时间：{DateTime.Now}\n");
			file.StoreString("=====================================\n");
			file.StoreString(content);
			file.Close();
		}
		catch (Exception e)
		{
			GD.PrintErr($"保存节点树文件失败：{e.Message}");
		}
	}

	// 按钮点击触发的方法（适配Control节点的UI交互）
	private void OnPrintButtonPressed()
	{
		// 可指定从"Control"父节点开始遍历
		PrintNodeTree(GetParent()); 
		EmitSignal(SignalName.TreePrinted);
	}

	// 自定义信号：节点树输出完成
	[Signal]
	public delegate void TreePrintedEventHandler();
}
