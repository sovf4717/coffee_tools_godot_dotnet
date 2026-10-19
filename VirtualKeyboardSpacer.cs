#if GODOT_MOBILE
using System;
#endif

using Godot;

public partial class VirtualKeyboardSpacer : MarginContainer
{
	#if GODOT_MOBILE
	public override void _Process(double delta)
	{
		var margin = (double)DisplayServer.VirtualKeyboardGetHeight();
		margin /= DisplayServer.ScreenGetScale();
		margin -= GetViewportRect().End.Y - GetGlobalRect().End.Y;
		AddThemeConstantOverride("margin_bottom", Math.Max(Mathf.FloorToInt(margin), 0));
	}
	#endif
}
