using Godot;
using System;

public partial class main : Node2D
{
	private GameInstance LoadedGame;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.LoadedGame = utils.LoadGame();
		if (LoadedGame == null)
		{
			this.GetNode("LoadButton").QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_quit_pressed()
	{
		this.GetTree().Quit();
	}

	private void _on_play_pressed()
	{
		gameplay.Instance = new GameInstance();
		this.GetTree().ChangeSceneToFile("res://game.tscn");
	}

	private void _on_load_button_pressed()
	{
		gameplay.Instance = LoadedGame;
		this.GetTree().ChangeSceneToFile("res://game.tscn");
	}

	private void _on_background_direction_toggled(bool toggled_on)
	{
		var background = GetNode<background_menu>("background_menu");
		background.movingRight = !toggled_on;
	}
}






