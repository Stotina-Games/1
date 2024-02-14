class_name gem
extends Area2D

var AddsGold: int = 20;
var consumed: bool;

func _ready():
	get_node("AnimatedSprite2D").play("idle");

func _on_body_entered(body: Node2D):
	if self.consumed:
		return
	if body.name != "player":
		return
	self.consumed = true;
	gameplay.ChangePlayerGold(self.AddsGold);

	var tweenPosition = get_tree().create_tween();
	tweenPosition.tween_property(self, "position", position + Vector2(-20, -50), 0.25);
	tweenPosition.tween_property(self, "position", position + Vector2(-40, 100), 0.5);

	var tweenRotation = get_tree().create_tween();
	tweenRotation.tween_property(self, "rotation", rotation - 100, 0.25);
	tweenRotation.tween_property(self, "rotation", rotation - 300, 0.1);
	tweenRotation.tween_property(self, "rotation", rotation - 500, 0.1);
	tweenRotation.tween_property(self, "rotation", rotation - 700, 0.1);
	tweenRotation.tween_property(self, "rotation", rotation - 900, 0.1);
	tweenRotation.tween_property(self, "rotation", rotation - 1100, 0.1);

	await tweenPosition.step_finished;

	var tweenAlpha = get_tree().create_tween();
	tweenAlpha.tween_property(self, "modulate:a", 1, 0.25);
	tweenAlpha.tween_property(self, "modulate:a", 0, 0.5);

	await tweenPosition.finished;
	await tweenRotation.finished;
	await tweenAlpha.finished;

	self.queue_free();

