using Godot;
using System;
using System.Text;

public partial class Label_HP : Label
{
	const string Heart = "♥️";
	const string EmptyHeart = "♡";
	const int MaxHearts = 20;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int maxHearts = Math.Min(MaxHearts, gameplay.Instance.PlayerMaxHP);

		float hpPercent = (float)gameplay.Instance.PlayerHP / gameplay.Instance.PlayerMaxHP;
		var fullHearts = Math.Floor((maxHearts * hpPercent) + 0.5);
		var sb = new StringBuilder("HP: ");
		for (int i = 0; i < fullHearts; i++) sb.Append(Heart);
		for (int i = 0; i < maxHearts - fullHearts; i++) sb.Append(EmptyHeart);

		sb.Append("  [");
		sb.Append(gameplay.Instance.PlayerHP);
		sb.Append("/");
		sb.Append(gameplay.Instance.PlayerMaxHP);
		sb.Append("]");


		sb.Append("  [");
		sb.Append((int)(hpPercent * 100));
		sb.Append("%]");

		sb.Append("  [");
		sb.Append(fullHearts);
		sb.Append(Heart);
		sb.Append("/");
		sb.Append(maxHearts);
		sb.Append(EmptyHeart);
		sb.Append("]");

		Text = sb.ToString();
	}
}
