using Godot;
using System;

public partial class KeyboardHandler : Control
{
	// 根容器（需要偏移的整个界面，对应节点树中的 "center"）
	[Export] private Control 根容器;
	// 滚动容器（日志标签页的滚动容器，可选）
	[Export] private ScrollContainer 滚动容器;
	
	// 目标输入框：下次制作建议
	private TextEdit 下次制作建议输入框;
	// 目标输入框：备注
	private TextEdit 备注输入框;
	
	// 记录原始位置和尺寸，用于恢复
	private Vector2 根容器原始位置;
	private Vector2 滚动容器原始尺寸;
	// 记录键盘状态和高度
	private bool 键盘可见 = false;
	private float 键盘高度 = 0;
	private bool 是目标输入框触发 = false;

	public override void _Ready()
	{
		// 初始化根容器（优先使用编辑器指定，否则自动查找 "center" 节点）
		if (根容器 == null)
		{
			根容器 = GetNodeOrNull<Control>("center");
			if (根容器 == null)
			{
				GD.PrintErr("未找到根容器，请在编辑器中指定 'center' 节点！");
				return;
			}
		}
		根容器原始位置 = 根容器.Position;

		// 初始化滚动容器（日志标签页的 ScrollContainer，可选）
		if (滚动容器 == null)
		{
			// 从节点树查找日志标签页的滚动容器（可选，根据需要启用）
			滚动容器 = GetNodeOrNull<ScrollContainer>("center/TabContainer/日志/ScrollContainer");
		}
		if (滚动容器 != null)
		{
			滚动容器原始尺寸 = 滚动容器.Size;
		}

		// 修正目标输入框的节点路径（根据你的节点树）
		下次制作建议输入框 = GetNodeOrNull<TextEdit>("center/TabContainer/计算/Control/下次制作建议");
		备注输入框 = GetNodeOrNull<TextEdit>("center/TabContainer/计算/Control/备注");

		// 容错处理
		if (下次制作建议输入框 == null)
			GD.PrintErr("未找到节点：center/TabContainer/计算/Control/下次制作建议");
		if (备注输入框 == null)
			GD.PrintErr("未找到节点：center/TabContainer/计算/Control/备注");

		// 绑定输入框聚焦/失焦信号
		if (下次制作建议输入框 != null)
		{
			下次制作建议输入框.FocusEntered += OnTargetInputFocused;
			下次制作建议输入框.FocusExited += OnTargetInputUnfocused;
		}
		if (备注输入框 != null)
		{
			备注输入框.FocusEntered += OnTargetInputFocused;
			备注输入框.FocusExited += OnTargetInputUnfocused;
		}
	}

	/// <summary>
	/// 目标输入框获取焦点时触发
	/// </summary>
	private void OnTargetInputFocused()
	{
		是目标输入框触发 = true;
	}

	/// <summary>
	/// 目标输入框失去焦点时触发
	/// </summary>
	private void OnTargetInputUnfocused()
	{
		if (是目标输入框触发 && !键盘可见)
		{
			恢复界面位置();
			是目标输入框触发 = false;
		}
	}

	/// <summary>
	/// 监听输入事件，检测虚拟键盘显示/隐藏
	/// </summary>
	public override void _Input(InputEvent @event)
	{
		// 仅处理目标输入框激活时的键盘事件
		if (!是目标输入框触发)
			return;

		// 检测虚拟键盘显示（触摸或鼠标事件触发时判断）
		if (@event is InputEventScreenTouch || @event is InputEventMouseButton)
		{
			// 间接判断：屏幕高度变化视为键盘弹出/隐藏
			float 屏幕高度 = GetViewportRect().Size.Y;
			float 预期正常高度 = GetViewportRect().Size.Y; // 用视口高度作为基准

			// 键盘弹出：屏幕高度减少超过20%
			if (屏幕高度 < 预期正常高度 * 0.8f && !键盘可见)
			{
				键盘高度 = 预期正常高度 - 屏幕高度;
				键盘可见 = true;
				调整界面位置();
			}
			// 键盘隐藏：屏幕高度恢复95%以上
			else if (屏幕高度 >= 预期正常高度 * 0.95f && 键盘可见)
			{
				键盘可见 = false;
				恢复界面位置();
			}
		}
	}

	/// <summary>
	/// 键盘弹出时调整界面位置
	/// </summary>
	private void 调整界面位置()
	{
		// 计算偏移距离（键盘高度的70%，避免过度偏移）
		float 偏移距离 = 键盘高度 * 0.7f;
		根容器.Position = 根容器原始位置 - new Vector2(0, 偏移距离);

		// 调整滚动容器高度（如果存在）
		if (滚动容器 != null)
		{
			滚动容器.Size = new Vector2(
				滚动容器原始尺寸.X,
				滚动容器原始尺寸.Y - 偏移距离
			);
		}
	}

	/// <summary>
	/// 键盘隐藏时恢复界面位置
	/// </summary>
	private void 恢复界面位置()
	{
		根容器.Position = 根容器原始位置;
		if (滚动容器 != null)
			滚动容器.Size = 滚动容器原始尺寸;
		键盘高度 = 0;
	}
}
