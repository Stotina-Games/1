using Godot;
using System;
using System.Text;

public partial class Label_Position : Label
{
	private player _player;
	private player GetPlayer()
	{
		if (_player != null) return _player;
		_player = GetNode<player>("../../player/player");
		return _player;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var player = GetPlayer();

		var sb = new StringBuilder("Position: { X: ");
		sb.Append(player.Position.X);
		sb.Append(", Y: ");
		sb.Append(player.Position.Y);
		sb.Append(" }");
		Text = sb.ToString();
	}
}
