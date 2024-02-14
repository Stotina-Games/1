class_name background_menu
extends ParallaxBackground

var movingRight: bool = false;
var scrollingSpeed: int = 100;

func _ready():
	pass

func _process(delta):
	if movingRight:
		self.scroll_offset.x += scrollingSpeed * delta;
	else:
		self.scroll_offset.x -= scrollingSpeed * delta;
