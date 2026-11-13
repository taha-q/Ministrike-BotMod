using System;
using ChartboostSDK;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

public class Cloud : MonoBehaviour
{
	private static SaveData saveData;

	private static ISavedGameMetadata savedGameMetadata;

	public void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		if (UnityEngine.Object.FindObjectsOfType(GetType()).Length > 1)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Start()
	{
		if (!Social.localUser.authenticated)
		{
			PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
			PlayGamesPlatform.InitializeInstance(configuration);
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesPlatform.Activate();
			Login();
		}
		else
		{
			Debug.LogError("Cloud.Start: Social.localUser already authenticated");
		}
	}

	public void Login()
	{
		if (!Social.localUser.authenticated)
		{
			Debug.Log("Cloud.Login");
			Social.localUser.Authenticate((bool success) =>
			{
				Debug.Log(Social.localUser.userName + " logged to google play games, loading game save");
				OpenSave(true);
			});
		}
		else
		{
			Debug.LogError("Cloud.Login: Social.localUser already authenticated");
		}
	}

	private void OpenSave(bool load)
	{
		ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
		savedGame.OpenWithAutomaticConflictResolution("savedata", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, (SavedGameRequestStatus status, ISavedGameMetadata game) =>
		{
			Debug.Log("OpenWithAutomaticConflictResolution: " + status);
			if (status == SavedGameRequestStatus.Success)
			{
				Debug.Log("game save opened");
				savedGameMetadata = game;
				if (load)
				{
					LoadSave();
				}
			}
			else
			{
				Debug.LogError("couldn't open game save");
				savedGameMetadata = null;
			}
		});
	}

	private void LoadSave()
	{
		if (savedGameMetadata != null)
		{
			Debug.Log("loading game save");
			ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
			savedGame.ReadBinaryData(savedGameMetadata, (SavedGameRequestStatus status, byte[] data) =>
			{
				Debug.Log("ReadBinaryData: " + status);
				if (status == SavedGameRequestStatus.Success)
				{
					try
					{
						saveData = SaveData.FromByteArray(data);
						if (saveData == null)
						{
							Debug.Log("saveData is null, assuming new user");
							saveData = new SaveData();
						}
						Debug.Log("game save loaded");
						Debug.Log(saveData.ToString());
						return;
					}
					catch (Exception)
					{
						saveData = null;
						Debug.LogError("couldn't load game save #3");
						return;
					}
				}
				Debug.LogError("couldn't load game save #2");
			});
		}
		else
		{
			Debug.LogError("couldn't load game save #1");
		}
	}

	public void Save()
	{
		if (!Social.localUser.authenticated)
		{
			Debug.LogError("couldn't upload game save #1 (!Social.localUser.authenticated)");
			return;
		}
		if (savedGameMetadata == null)
		{
			Debug.LogError("couldn't upload game save #1 (savedGameMetadata == null)");
			return;
		}
		if (saveData == null)
		{
			Debug.LogError("couldn't upload game save #1 (saveData == null)");
			return;
		}
		Debug.Log("uploading game save");
		Debug.Log(saveData.ToString());
		ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
		SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedPlayedTime(savedGameMetadata.TotalTimePlayed.Add(new TimeSpan(0, 0, (int)Time.realtimeSinceStartup))).WithUpdatedDescription("Saved game at " + DateTime.Now).Build();
		savedGame.CommitUpdate(savedGameMetadata, updateForMetadata, SaveData.ToByteArray(saveData), (SavedGameRequestStatus status, ISavedGameMetadata game) =>
		{
			Debug.Log("CommitUpdate: " + status);
			if (status == SavedGameRequestStatus.Success)
			{
				Debug.Log("game save uploaded");
				savedGameMetadata = game;
				if (!savedGameMetadata.IsOpen)
				{
					OpenSave(false);
				}
			}
			else
			{
				Debug.LogError("couldn't upload game save #2");
				savedGameMetadata = null;
			}
		});
	}

	public void SaveLB()
	{
		if (Social.localUser.authenticated && savedGameMetadata != null && saveData != null)
		{
			Debug.Log(string.Concat("leaderboard: uploading kills (", saveData.kills, ")"));
			Social.ReportScore((int)saveData.kills, "CgkI86WmwoIHEAIQBg", (bool success) =>
			{
				Debug.Log(string.Concat("leaderboard: kills upload=", success, " (", saveData.kills, ")"));
			});
			Debug.Log(string.Concat("leaderboard: uploading deaths (", saveData.deaths, ")"));
			Social.ReportScore((int)saveData.deaths, "CgkI86WmwoIHEAIQBw", (bool success) =>
			{
				Debug.Log(string.Concat("leaderboard: deaths upload=", success, " (", saveData.deaths, ")"));
			});
		}
	}

	public void ShowLeaderBoards()
	{
		Debug.Log("Cloud.ShowLeaderBoards");
		if (Social.localUser.authenticated)
		{
			Debug.Log("Social.ShowLeaderboardUI");
			Social.ShowLeaderboardUI();
		}
	}

	public void AddKill()
	{
		if (saveData != null)
		{
			saveData.kills += 1;
		}
	}

	public void AddDeath()
	{
		if (saveData != null)
		{
			saveData.deaths += 1;
		}
	}

	public void ShowAdChartboost()
	{
		Debug.Log("ShowAdChartboost: Chartboost.showInterstitial");
		Chartboost.showInterstitial(CBLocation.HomeScreen);
	}
}
