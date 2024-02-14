extends Label

func _process(_delta):
	var pl = get_node("../../player/player") as CharacterBody2D;
	text = "".join([
		"Position: { X: ",
		pl.position.x,
		", Y: ",
		pl.position.y,
		" }"
	]);
