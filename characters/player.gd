class_name player
extends CharacterBody2D

var MaxSpeed:float = 400.0;
var AccelerationPower:float = 800.0;
var AccelerationPowerInAir:float = 700.0;
var JumpVelocity:float = -400.0;
var Friction:float = 600.0;

var HurtDurationMS:int = 500;

var animationLocked:bool = false;
var forcedToJump:bool = false;
var isAlive:bool = true;

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity");

var _anim: AnimationPlayer;
var _sprite: AnimatedSprite2D;

func GetSprite() -> AnimatedSprite2D:
	if _sprite == null:
		_sprite = get_node("AnimatedSprite2D");
	return _sprite;

func GetAnim() -> AnimationPlayer:
	if _anim == null:
		_anim = get_node("AnimationPlayer");
	return _anim;

func SetAnimation(animationName):
	if !animationLocked:
		GetAnim().play(animationName);

func SetSpriteDirection(lookLeft: bool):
	GetSprite().flip_h = lookLeft

func _ready():
	SetAnimation("idle");

func _physics_process(delta: float) -> void:
	if !isAlive:
		return;

	# Add the gravity.
	var isOnFloor: bool = is_on_floor();
	if (!isOnFloor):
		velocity.y += gravity * delta;
	else:
		velocity.y = 0;

	var prev_velocity = Vector2(velocity);

	# Handle Jump.
	if self.forcedToJump:
		velocity.y = JumpVelocity;
		forcedToJump = false;
	elif Input.is_action_just_pressed("ui_accept") && isOnFloor:
		velocity.y = JumpVelocity;

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction: Vector2 = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down");
#
	if direction.x == -1:
		SetSpriteDirection(true);
	elif direction.x == 1:
		SetSpriteDirection(false);

	var acceleration: float = self.AccelerationPower if isOnFloor else self.AccelerationPowerInAir;
	var normalizedVelocity: Vector2 = velocity.normalized();
#
	if (direction.x != 0):
		velocity.x -= normalizedVelocity.x * Friction * delta; # reduce prev speed with friction
		velocity.x += direction.x * acceleration * delta; # add new acceleration;
		if (absf(velocity.x) > MaxSpeed):
			velocity.x = direction.x * MaxSpeed; # limit acceleration
	elif (absf(velocity.x) > Friction * delta):
		velocity.x -= normalizedVelocity.x * Friction * delta; # reduce prev speed with friction
	else:
		velocity.x = 0;

	if (isOnFloor):
		if (velocity.x != 0):
			# TODO: this is checking if player is slowing down. Consider adding animation for that.
			if (abs(prev_velocity.x) > abs(velocity.x)):
				SetAnimation("run"); # Using 'run' for now
			else:
				SetAnimation("run");
		else:
			SetAnimation("idle");
	elif (velocity.y < 0):
		SetAnimation("jump");
	elif (velocity.y > 0):
		SetAnimation("fall");

	move_and_slide();

func DealDamage(damage: int, jumpAwayFromPosition: Vector2):
	if (!isAlive):
		return;
	gameplay.ChangePlayerHP(-damage);
	if (gameplay.Instance.PlayerHP > 0):
		ForcePlayerToJump(jumpAwayFromPosition);
	SetAnimation("hurt");
	animationLocked = true;
	if (gameplay.Instance.PlayerHP <= 0):
		await KillPlayer();
	else:
		await GetAnim().animation_finished
		animationLocked = false;

func KillPlayer():
	if !isAlive:
		return
	isAlive = false;

	# allows character to fall through the floor
	(get_node("CollisionShape2D") as CollisionShape2D).set_deferred("disabled", true);

	var tweenPosition = get_tree().create_tween();
	tweenPosition.tween_property(self, "position", self.position + Vector2(20, -20), 0.25);
	tweenPosition.tween_property(self, "position", self.position + Vector2(40, 150), 0.75);

	var tweenRotation = get_tree().create_tween();
	tweenRotation.tween_property(self, "rotation", 90, 0.25);
	tweenRotation.tween_property(self, "rotation", 270, 0.75);

	await tweenPosition.finished;
	await tweenRotation.finished;

	await get_tree().create_timer(0.5).timeout;

	queue_free();
	get_tree().change_scene_to_file("res://main.tscn");


func ForcePlayerToJump(jumpAwayFromPosition: Vector2):
	if (!isAlive):
		return;
	var _direction = (global_position - jumpAwayFromPosition).normalized();
	forcedToJump = true;
