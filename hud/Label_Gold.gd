extends Label

func _process(_delta):
	text = "".join(["Gold: ", gameplay.Instance.PlayerGold]);
