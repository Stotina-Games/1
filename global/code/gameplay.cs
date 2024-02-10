using Godot;
using System;

public partial class gameplay : Node
{
	public static GameInstance Instance = new GameInstance();

	public override void _Ready()
	{
	}

	public static void ChangePlayerHP(int hpIncrement)
	{
		Instance.PlayerHP += hpIncrement;
		if (Instance.PlayerHP > Instance.PlayerMaxHP) Instance.PlayerHP = Instance.PlayerMaxHP;
		if (Instance.PlayerHP < 0) Instance.PlayerHP = 0;

		if (Instance.PlayerHP > 0)
			utils.SaveGame();
	}
	public static void ChangePlayerGold(int goldIncrement)
	{
		Instance.PlayerGold += goldIncrement;
		if (Instance.PlayerGold < 0) Instance.PlayerGold = 0;

		utils.SaveGame();
	}
}
