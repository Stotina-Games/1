using Godot;
using System;
using System.Threading.Tasks;

public partial class cherry : Area2D
{
	private int RestoresHP = 2;
	private bool consumed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void _on_body_entered(Node2D body)
	{
		if (!consumed)
		{
			if (body.Name == "player")
			{
				consumed = true;
				gameplay.ChangePlayerHP(this.RestoresHP);

				var tweenPosition = GetTree().CreateTween();
				tweenPosition.TweenProperty(this, "position", Position + new Vector2(20, -50), 0.25);
				tweenPosition.TweenProperty(this, "position", Position + new Vector2(40, 100), 0.5);

				var tweenRotation = GetTree().CreateTween();
				tweenRotation.TweenProperty(this, "rotation", Rotation + 100, 0.25);
				tweenRotation.TweenProperty(this, "rotation", Rotation + 300, 0.1);
				tweenRotation.TweenProperty(this, "rotation", Rotation + 500, 0.1);
				tweenRotation.TweenProperty(this, "rotation", Rotation + 700, 0.1);
				tweenRotation.TweenProperty(this, "rotation", Rotation + 900, 0.1);
				tweenRotation.TweenProperty(this, "rotation", Rotation + 1100, 0.1);

				await ToSignal(tweenPosition, Tween.SignalName.StepFinished);

				var tweenAlpha = GetTree().CreateTween();
				tweenAlpha.TweenProperty(this, "modulate:a", 1, 0.5);
				tweenAlpha.TweenProperty(this, "modulate:a", 0, 0.25);

				await ToSignal(tweenPosition, Tween.SignalName.Finished);
				await ToSignal(tweenRotation, Tween.SignalName.Finished);
				await ToSignal(tweenAlpha, Tween.SignalName.Finished);

				this.QueueFree();
			}
		}
	}

}
