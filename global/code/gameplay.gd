class_name gameplay
extends Node

const SAVE_PATH_DEV = "res://save/savegame.json"
const SAVE_PATH_USER = "user://save/savegame.json"
const SAVE_PATH = SAVE_PATH_DEV

static var Instance = GameInstance.new();

static func ChangePlayerHP(hpIncrement: int):
	Instance.PlayerHP += hpIncrement;
	if Instance.PlayerHP > Instance.PlayerMaxHP:
		Instance.PlayerHP = Instance.PlayerMaxHP;
	if Instance.PlayerHP < 0:
		Instance.PlayerHP = 0;
	if Instance.PlayerHP > 0:
		SaveGame();

static func ChangePlayerGold(goldIncrement: int):
	Instance.PlayerGold += goldIncrement;
	if Instance.PlayerGold < 0:
		Instance.PlayerGold = 0;
	SaveGame();

static func SaveGame():
	var file = FileAccess.open(SAVE_PATH, FileAccess.WRITE)
	if file == null:
		var err = FileAccess.get_open_error();
		print("SaveGame Error: " + str(err));
		return;
	var data = {
		"PlayerMaxHP": gameplay.Instance.PlayerMaxHP,
		"PlayerHP": gameplay.Instance.PlayerHP,
		"PlayerGold": gameplay.Instance.PlayerGold,
	};
	var jsonString = JSON.stringify(data);
	file.store_string(jsonString)


static func  LoadGame() -> GameInstance:
	var file = FileAccess.open(SAVE_PATH, FileAccess.READ)
	if file == null:
		return null
	if !FileAccess.file_exists(SAVE_PATH) || file.eof_reached():
		return null
	var jsonString = file.get_as_text()
	if !jsonString || !jsonString.length():
		return null
	var data: Dictionary = JSON.parse_string(jsonString)
	var gameInstance: GameInstance = GameInstance.new()

	gameInstance.PlayerMaxHP = int(data.PlayerMaxHP);
	gameInstance.PlayerHP = int(data.PlayerHP);
	gameInstance.PlayerGold = int(data.PlayerGold);
	return gameInstance;
