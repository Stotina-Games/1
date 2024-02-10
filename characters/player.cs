using Godot;
using System;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public partial class player : CharacterBody2D
{
	public const float MaxSpeed = 400.0f;
	public const float AccelerationPower = 800.0f;
	public const float AccelerationPowerInAir = 700.0f;
	public const float JumpVelocity = -400.0f;
	public const float Friction = 600.0f;

	public const int HurtDurationMS = 500;

	public bool animationLocked = false;
	public bool forcedToJump = false;
	public bool isAlive = true;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private AnimationPlayer _anim;
	private AnimatedSprite2D _sprite;

	public AnimatedSprite2D GetSprite()
	{
		if (_sprite == null) _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		return _sprite;
	}
	public AnimationPlayer GetAnim()
	{
		if (_anim == null) _anim = GetNode<AnimationPlayer>("AnimationPlayer");
		return _anim;
	}

	public void SetAnimation(string animationName)
	{
		if (!animationLocked)
			GetAnim().Play(animationName);
	}

	public void SetSpriteDirection(bool lookLeft)
	{
		GetSprite().FlipH = lookLeft;
	}

	public override void _Ready()
	{
		base._Ready();
		SetAnimation("idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;

		float fDelta = (float)delta;

		Vector2 velocity = Velocity;

		// Add the gravity.
		bool isOnFloor = IsOnFloor();
		if (!isOnFloor) { velocity.Y += gravity * fDelta; }
		else { velocity.Y = 0; }

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && isOnFloor)
		{
			velocity.Y = JumpVelocity;
		}
		if (forcedToJump)
		{
			velocity.Y = JumpVelocity;
			forcedToJump = false;
		}


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (direction.X == -1) SetSpriteDirection(true);
		else if (direction.X == 1) SetSpriteDirection(false);

		float acceleration = isOnFloor ? AccelerationPower : AccelerationPowerInAir;
		var normalizedVelocity = velocity.Normalized();

		if (direction.X != 0)
		{
			velocity.X -= normalizedVelocity.X * Friction * fDelta; // reduce prev speed with friction
			velocity.X += direction.X * acceleration * fDelta; // add new acceleration;
			if (Math.Abs(velocity.X) > MaxSpeed) velocity.X = direction.X * MaxSpeed; // limit acceleration
		}
		else if (Math.Abs(velocity.X) > Friction * delta)
		{
			velocity.X -= normalizedVelocity.X * Friction * fDelta; // reduce prev speed with friction
		}
		else
		{
			velocity.X = 0;
		}

		if (isOnFloor)
		{
			if (velocity.X != 0)
			{
				// TODO: this is checking if player is slowing down. Consider adding animation for that.
				if (Math.Abs(Velocity.X) > Math.Abs(velocity.X)) SetAnimation("run"); // Using 'run' for now
				else SetAnimation("run");
			}
			else SetAnimation("idle");
		}
		else if (velocity.Y < 0) SetAnimation("jump");
		else if (velocity.Y > 0) SetAnimation("fall");

		Velocity = velocity;
		MoveAndSlide();
	}

	public async void DealDamage(int damage, Vector2 position)
	{
		if (!isAlive) return;

		gameplay.ChangePlayerHP(-damage);

		if (gameplay.Instance.PlayerHP > 0)
		{
			ForcePlayerToJump(position);
		}

		GD.Print("> Playing (Locked) Animation: hurt");
		SetAnimation("hurt");
		animationLocked = true;

		if (gameplay.Instance.PlayerHP <= 0)
		{
			await KillPlayer();
		}
		else
		{
			await Task.Delay(HurtDurationMS);
			animationLocked = false;
			GD.Print("< Done Playing (Locked) Animation: hurt");
		}
	}

	private async Task KillPlayer()
	{
		this.isAlive = false;
		// this.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true); // allows character to fall through the floor

		var tweenPosition = GetTree().CreateTween();
		tweenPosition.TweenProperty(this, "position", this.Position + new Vector2(20, -20), 0.25);
		tweenPosition.TweenProperty(this, "position", this.Position + new Vector2(40, 150), 0.75);

		var tweenRotation = GetTree().CreateTween();
		tweenRotation.TweenProperty(this, "rotation", 90, 0.25);
		tweenRotation.TweenProperty(this, "rotation", 270, 0.75);

		await ToSignal(tweenPosition, Tween.SignalName.Finished);

		await Task.Delay(500);

		this.QueueFree();
		this.GetTree().ChangeSceneToFile("res://main.tscn");
	}

	internal void ForcePlayerToJump(Vector2 pushPosition)
	{
		if (!isAlive) return;
		var direction = (this.GlobalPosition - pushPosition).Normalized();
		this.forcedToJump = true;
	}
}
