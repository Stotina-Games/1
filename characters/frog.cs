using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class frog : CharacterBody2D
{
	public int Damage = 1;

	public float MaxSpeed = 80.0f;
	public float Acceleration = 200.0f;
	public float Friction = 120.0f;
	public float JumpVelocity = -250.0f;
	public int MobGold = 5;

	public RandomNumberGenerator rnd = new RandomNumberGenerator();

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();


	public bool isChasing = false;
	public bool mobPausedAfterDamagingThePlayer = false;
	public bool isAlive = true;
	public bool animationLocked = false;

	private AnimatedSprite2D _sprite;

	public AnimatedSprite2D GetSprite()
	{
		if (_sprite == null) _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		return _sprite;
	}

	public void SetAnimation(string animationName)
	{
		if (!animationLocked)
			GetSprite().Play(animationName);
	}

	public void SetSpriteDirection(bool lookLeft)
	{
		GetSprite().FlipH = lookLeft;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetSprite().Play("idle");
		StartSpawnSequence();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		float fDelta = (float)delta;
		var normalizedVelocity = velocity.Normalized();
		var isOnFloor = IsOnFloor();


		// Movement
		if (isAlive)
		{
			// Gravity
			if (!isOnFloor) velocity.Y += gravity * (float)delta;


			if (isChasing && !mobPausedAfterDamagingThePlayer)
			{
				if (isOnFloor) velocity.Y = JumpVelocity; // Frogs always jump

				velocity.X -= normalizedVelocity.X * Friction * fDelta; // reduce prev speed with friction

				var direction = (GetPlayer().GlobalPosition - GlobalPosition).Normalized();
				if (direction.X != 0)
				{
					velocity.X += direction.X * Acceleration * fDelta; // add new acceleration;
					if (Math.Abs(velocity.X) > MaxSpeed) velocity.X = direction.X * MaxSpeed; // limit acceleration
					SetSpriteDirection(direction.X > 0);
				}
				if (Math.Abs(velocity.X) < Friction * delta)
				{
					GetSprite().Play("idle");
					velocity.X = 0;
				}
			}
			else { GetSprite().Play("idle"); }

			if (velocity.Y < 0) GetSprite().Play("jump");
			else if (velocity.Y > 0) GetSprite().Play("fall");
		}

		Velocity = velocity;

		MoveAndSlide();
	}

	public async void StartSpawnSequence()
	{

		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
		GetSprite().Play("spawn");
		this.isAlive = false;
		animationLocked = true;

		await ToSignal(GetSprite(), AnimatedSprite2D.SignalName.AnimationLooped);

		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
		this.isAlive = true;
		animationLocked = false;
		GetSprite().Play("idle");

	}

	private player _player;
	private player GetPlayer()
	{
		if (_player != null) return _player;
		_player = GetNode<player>("../../player/player");
		return _player;
	}

	private void _on_detection_body_entered(Node2D body)
	{
		if (body.Name == "player")
			isChasing = true;
	}

	private void _on_detection_body_exited(Node2D body)
	{
		if (isAlive)
			if (body.Name == "player")
				isChasing = false;
	}

	private async void _on_damage_dealing_area_body_entered(Node2D body)
	{
		if (isAlive)
		{
			if (body.Name == "player")
			{
				mobPausedAfterDamagingThePlayer = true;
				GetPlayer().DealDamage(this.Damage, this.Position);
				GetSprite().Play("idle");
				await ToSignal(GetSprite(), "animation_looped");
				mobPausedAfterDamagingThePlayer = false;
			}
		}
	}


	private async void _on_mob_kill_area_body_entered(Node2D body)
	{
		if (isAlive)
		{
			if (body.Name == "player")
			{
				var player = body as CharacterBody2D;
				if (player.Velocity.Y < -100)
				{
					return; // assuming that the player passed through the frog from bellow. Not a valid kill.
				}

				gameplay.ChangePlayerGold(MobGold);
				isChasing = false;
				isAlive = false;
				this.Velocity = Vector2.Zero;
				GetPlayer().ForcePlayerToJump(this.Position);

				// remove boxes to turn mob into a visual-only thing

				GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
				GetNode<CollisionShape2D>("Detection/CollisionShape2D").SetDeferred("disabled", true);
				GetNode<CollisionShape2D>("MobKillArea/CollisionShape2D").SetDeferred("disabled", true);
				GetNode<CollisionShape2D>("DamageDealingArea/CollisionShape2D").SetDeferred("disabled", true);

				var tweenPosition = GetTree().CreateTween();
				tweenPosition.TweenProperty(this, "position", this.Position + new Vector2(0, -20), 0.25);
				tweenPosition.TweenProperty(this, "position", this.Position, 0.5);

				var tweenRotation = GetTree().CreateTween();
				tweenRotation.TweenProperty(this, "rotation", 0, 0.25);
				tweenRotation.TweenProperty(this, "rotation", 90, 0.5);
				tweenRotation.TweenProperty(this, "rotation", 90, 0.25);

				await ToSignal(tweenPosition, Tween.SignalName.Finished);
				await ToSignal(tweenRotation, Tween.SignalName.Finished);

				GetSprite().Play("death");
				this.Rotation = 0;
				await ToSignal(GetSprite(), AnimatedSprite2D.SignalName.AnimationFinished);
				this.QueueFree();
			}
		}
	}

}




