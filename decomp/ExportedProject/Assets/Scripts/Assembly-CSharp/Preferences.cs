using UnityEngine;

public class Preferences
{
	private static string playerKey = "Mini";

	public static string GetPlayerName()
	{
		if (Social.localUser.authenticated)
		{
			return Social.localUser.userName;
		}
		return PlayerPrefs.GetString(playerKey, "Mini" + Random.Range(100, 999999));
	}

	public static void SetPlayerName(string name)
	{
		PlayerPrefs.SetString(playerKey, name);
	}
}
