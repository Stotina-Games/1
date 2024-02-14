class_name spawner
extends Area2D

static var PathToFrogScene = "res://characters/frog.tscn";
static var PathToCherryScene = "res://elements/cherry.tscn";
static var PathToGemScene = "res://elements/gem.tscn";

static var FrogScene: PackedScene = load(PathToFrogScene) as PackedScene;
static var CherryScene: PackedScene = load(PathToCherryScene) as PackedScene;
static var GemScene: PackedScene = load(PathToGemScene) as PackedScene;

static var rnd: RandomNumberGenerator = RandomNumberGenerator.new();

func _on_timer_timeout():
	var instance: Node2D;

	var i = rnd.randi_range(0, 12);
	if i == 0:
		instance = GemScene.instantiate() as Node2D;
	elif i == 1:
		instance = CherryScene.instantiate() as Node2D;
	else:
		instance = FrogScene.instantiate() as Node2D;
	instance.position = SelectRandomPosition(instance);
	add_child(instance);

func SelectRandomPosition(instance: Node2D) -> Vector2:
	var size: Vector2 = ((get_node("CollisionShape2D") as CollisionShape2D).shape as RectangleShape2D).size;
	return instance.position + Vector2(
		rnd.randf_range(-size.x / 2, size.x / 2),
		rnd.randf_range(-size.y / 2, size.y / 2)
	);
