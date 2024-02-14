class_name frog
extends CharacterBody2D

var Damage: int = 1;

var MaxSpeed: float = 80.0;
var Acceleration: float = 200.0;
var Friction: float = 120.0;
var JumpVelocity: float = -250.0;
var MobGold: int = 5;

var rnd: RandomNumberGenerator = RandomNumberGenerator.new();

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity");

var isChasing: bool = false;
var mobPausedAfterDamagingThePlayer: bool = false;
var isAlive: bool = true;
var animationLocked: bool = false;

var _sprite: AnimatedSprite2D;

func GetSprite() -> AnimatedSprite2D:
	if (_sprite == null):
		_sprite = get_node("AnimatedSprite2D");
	return _sprite;

func SetAnimation(animationName):
	if (!animationLocked):
		GetSprite().play(animationName);

func SetSpriteDirection(lookLeft: bool):
	GetSprite().flip_h = lookLeft;

var _player: player;
func GetPlayer() -> player:
	if !_player:
		_player = get_node("../../player/player");
	return _player;

func _ready():
	GetSprite().play("idle");
	StartSpawnSequence();

func _physics_process(delta: float) -> void:
	var normalizedVelocity = velocity.normalized();
	var isOnFloor = is_on_floor();

	# Movement
	if (isAlive):
		# Gravity
		if (!isOnFloor):
			velocity.y += gravity * delta;

		if (isChasing && !mobPausedAfterDamagingThePlayer):
			if (isOnFloor):
				velocity.y = JumpVelocity; # Frogs always jump
#
			velocity.x -= normalizedVelocity.x * Friction * delta; # reduce prev speed with friction
#
			var direction = (GetPlayer().global_position - global_position).normalized();
			if (direction.x != 0):
				velocity.x += direction.x * Acceleration * delta; # add new acceleration;
				if (abs(velocity.x) > MaxSpeed):
					velocity.x = direction.x * MaxSpeed; # limit acceleration
				SetSpriteDirection(direction.x > 0);
			if (abs(velocity.x) < Friction * delta):
				GetSprite().play("idle");
				velocity.x = 0;
		else:
			GetSprite().play("idle");

		if (velocity.y < 0):
			GetSprite().play("jump");
		elif (velocity.y > 0):
			GetSprite().play("fall");

		move_and_slide();

func StartSpawnSequence():
	(get_node("CollisionShape2D") as CollisionShape2D).set_deferred("disabled", true);
	GetSprite().play("spawn");
	self.isAlive = false;
	animationLocked = true;

	await GetSprite().animation_looped;

	(get_node("CollisionShape2D") as CollisionShape2D).set_deferred("disabled", false);
	self.isAlive = true;
	animationLocked = false;
	GetSprite().play("idle");


func _on_detection_body_entered(body: Node2D):
	if (body.name == "player"):
		isChasing = true;

func _on_detection_body_exited( body: Node2D):
	if (isAlive):
		if (body.name == "player"):
			isChasing = false;

func _on_damage_dealing_area_body_entered(body: Node2D):
	if (!isAlive):
		return
	if (body.name != "player"):
		return
	mobPausedAfterDamagingThePlayer = true;
	GetPlayer().DealDamage(Damage, position);
	GetSprite().play("idle");
	await GetSprite().animation_looped;
	mobPausedAfterDamagingThePlayer = false;

func _on_mob_kill_area_body_entered(body: Node2D):
	if (!isAlive):
		return
	if (body.name != "player"):
		return
	var pl = body as CharacterBody2D;
	if (pl.velocity.y < -100):
		return; # assuming that the player passed through the frog from bellow. Not a valid kill.

	gameplay.ChangePlayerGold(MobGold);
	isChasing = false;
	isAlive = false;
	self.velocity = Vector2.ZERO;
	GetPlayer().ForcePlayerToJump(self.position);

	# remove boxes to turn mob into a visual-only thing
	(get_node("CollisionShape2D") as CollisionShape2D).set_deferred("disabled", true);
	(get_node("Detection/CollisionShape2D") as CollisionShape2D).set_deferred("disabled", true);
	(get_node("MobKillArea/CollisionShape2D") as CollisionShape2D).set_deferred("disabled", true);
	(get_node("DamageDealingArea/CollisionShape2D") as CollisionShape2D).set_deferred("disabled", true);
#
	var tweenPosition = get_tree().create_tween();
	tweenPosition.tween_property(self, "position", position + Vector2(0, -20), 0.25);
	tweenPosition.tween_property(self, "position", position, 0.5);
#
	var tweenRotation = get_tree().create_tween();
	tweenRotation.tween_property(self, "rotation", 0, 0.25);
	tweenRotation.tween_property(self, "rotation", 90, 0.5);
	tweenRotation.tween_property(self, "rotation", 90, 0.25);
#
	await tweenPosition.finished;
	await tweenRotation.finished;
#
	GetSprite().play("death");
	rotation = 0;
	await GetSprite().animation_finished;
	queue_free();
