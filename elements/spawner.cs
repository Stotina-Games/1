using Godot;
using System;
using System.Drawing;
using System.IO;

public partial class spawner : Area2D
{
	public const string PathToFrogScene = "res://characters/frog.tscn";
	public const string PathToCherryScene = "res://elements/cherry.tscn";
	public const string PathToGemScene = "res://elements/gem.tscn";

	public PackedScene FrogScene = GD.Load(PathToFrogScene) as PackedScene;
	public PackedScene CherryScene = GD.Load(PathToCherryScene) as PackedScene;
	public PackedScene GemScene = GD.Load(PathToGemScene) as PackedScene;

	public RandomNumberGenerator rnd = new RandomNumberGenerator();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_timer_timeout()
	{
		Node2D instance;

		var i = rnd.RandiRange(0, 12);
		if (i == 0)
		{
			instance = GemScene.Instantiate() as Node2D;
		}
		else if (i == 1)
		{
			instance = CherryScene.Instantiate() as Node2D;
		}
		else
		{
			instance = GemScene.Instantiate() as Node2D;

			// instance = FrogScene.Instantiate() as Node2D;
		}

		instance.Position = SelectRandomPosition(instance);
		AddChild(instance);
	}

	private Vector2 SelectRandomPosition(Node2D instance)
	{
		Vector2 size = (GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D).Size;
		return instance.Position + new Vector2(
					rnd.RandfRange(-size.X / 2, size.X / 2),
					rnd.RandfRange(-size.Y / 2, size.Y / 2)
				);
	}
}

