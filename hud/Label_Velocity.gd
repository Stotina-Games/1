extends Label

func _process(_delta):
	var pl = get_node("../../player/player") as CharacterBody2D;
	text = "".join([
		"Velocity: { X: ",
		pl.velocity.x,
		", Y: ",
		pl.velocity.y,
		" }"
	]);
