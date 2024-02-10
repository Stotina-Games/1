using Godot;
using System;

public partial class background_menu : ParallaxBackground
{
	public bool movingRight = false;
	public int scrollingSpeed = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 scrollOffset = ScrollOffset;
		float x = scrollOffset.X;
		if (movingRight) x = (scrollOffset.X + scrollingSpeed * (float)delta);
		else x = (scrollOffset.X - scrollingSpeed * (float)delta);
		scrollOffset.X = x;
		ScrollOffset = scrollOffset;
	}
}
