extends Node2D

var LoadedGame: GameInstance;

func _ready():
	LoadedGame = gameplay.LoadGame();
	if LoadedGame == null:
		get_node("LoadButton").queue_free();

func _on_quit_pressed():
	get_tree().quit();

func _on_play_pressed():
	gameplay.Instance = GameInstance.new();
	get_tree().change_scene_to_file("res://game.tscn");

func _on_load_button_pressed():
	gameplay.Instance = LoadedGame;
	get_tree().change_scene_to_file("res://game.tscn");

func _on_background_direction_toggled(toggled_on: bool):
	var background: background_menu = get_node("background_menu");
	background.movingRight = !toggled_on;
