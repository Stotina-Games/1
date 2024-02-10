using Godot;
using System;
using System.Collections.Generic;

public partial class utils : Node
{
	public const string SAVE_PATH = "res://save/savegame.json";

	public static void SaveGame()
	{
		var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.WriteRead);
		if (file == null) GD.Print("SaveGame Error: " + FileAccess.GetOpenError());

		using (file)
		{
			var data = new Dictionary<string, Object>
			{
				{ "PlayerHP", gameplay.Instance.PlayerHP },
				{ "PlayerGold", gameplay.Instance.PlayerGold }
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(data);
			file.StoreString(jsonString);
			GD.Print("Saved : " + jsonString + " to " + file.GetPath());
		}
	}

	public static GameInstance LoadGame()
	{
		var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
		if (file == null) GD.Print("LoadGame Error: " + FileAccess.GetOpenError());

		using (file)
		{
			if (FileAccess.FileExists(SAVE_PATH) && !file.EofReached())
			{
				var jsonString = file.GetAsText();
				if (!String.IsNullOrEmpty(jsonString))
				{
					var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Object>>(jsonString);
					if (data != null)
					{
						var gameInstance = new GameInstance();
						gameInstance.PlayerHP = int.Parse(data["PlayerHP"].ToString());
						gameInstance.PlayerGold = int.Parse(data["PlayerGold"].ToString());
						return gameInstance;
					}
				}
			}
		}

		return null;
	}
}

