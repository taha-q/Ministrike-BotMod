using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	public enum Team
	{
		Spectator = 0,
		Counter = 1,
		Terro = 2
	}

	public GameLogicRPC rpc;

	public UIPlayTween bloodTweener;

	public GameObject music;

	public Ui ui;

	public PlayerController controller;

	public NetworkDiscoveryEx networkDiscovery;

	public NetworkInfo networkInfo;

	public IpLocation location;

	public Cloud cloud;

	public int[] score;

	public int timeRemainingMap;

	public float startTimeMap;

	public int timeRemainingRound;

	public float startTimeRound;

	public bool restarting;

	public GameObject playerPrefab;

	public Transform playersHolder;

	public Transform droppedWeapons;

	public NetworkView netView;

	public bool isInitialized;

	public NetworkPlayerList players;

	public Config config;

	private bool exiting;

	private void Awake()
	{
		Application.targetFrameRate = 60;
		score = new int[3];
		rpc = GetComponent<GameLogicRPC>();
	}

	private void Start()
	{
		Debug.Log("#### GameLogic.Start(): config.Load() ####");
		ui.Show(Ui.UiState.None);
		if (SystemInfo.graphicsDeviceID != 0 || Application.platform == RuntimePlatform.Android)
		{
			ui.box.Show(UIMessageBox.UIMessageType.Infiny, "Please Wait", "Loading configuration and maps...");
		}
		config.Load((bool ended, string map, int progress) =>
		{
			Debug.Log("GameLogic.Start(): config.Load(" + ended + ", " + map + ", " + progress + ")");
			if (progress != 100)
			{
				ui.box.SetTextLabel("Loading configuration and maps...\n\nDownloading map: " + map + " (" + progress + "/100)");
			}
			else
			{
				Debug.Log("networkInfo.Load()");
				networkInfo.Load();
				Debug.Log("GameLogic.Start(): batchMode = " + config.server.batchMode);
				if (!config.server.batchMode)
				{
					ui.menuServers.UpdateMapList();
					ui.menuServers.gameObject.SetActive(true);
					ui.menuServers.tweenerMenuServer.ResetToBeginning();
					ui.menuServers.tweenerMenuServer.PlayForward();
					ui.menuServers.OnUpdateServers();
					ui.box.Hide();
					cloud.ShowAdChartboost();
				}
				ui.gameObject.SetActive(!config.server.batchMode);
			}
			Debug.Log("#### GameLogic.Start(): config.Load() ####");
		});
	}

	private void Update()
	{
		if (!isInitialized)
		{
			return;
		}
		timeRemainingRound = config.server.timePerRound - ((int)Time.time - (int)startTimeRound);
		timeRemainingMap = config.server.timePerMap - ((int)Time.time - (int)startTimeMap);
		if (!IsRestarting())
		{
			if (Network.isServer && timeRemainingRound <= 0)
			{
				Restart();
			}
			else if (Network.isServer && timeRemainingMap <= 0)
			{
				Restart(config.maps.GetNext().name);
			}
		}
	}

	public void Restart(string mapName = "none", float delay = 5f)
	{
		if (!Network.isServer || IsRestarting())
		{
			Debug.LogError("Restart(" + mapName + "): Network.isServer: " + Network.isServer + " | IsRestarting(): " + IsRestarting());
		}
		else
		{
			restarting = true;
			StartCoroutine(RestartRoutine(mapName, delay));
		}
	}

	public IEnumerator RestartRoutine(string mapName = "none", float delay = 5f)
	{
		if (mapName != "none")
		{
			netView.RPC("RPCMessageInfo", RPCMode.All, "Changing map in " + (int)delay + " secondes");
		}
		else
		{
			netView.RPC("RPCMessageInfo", RPCMode.All, "Restarting in " + (int)delay + " secondes");
		}
		yield return new WaitForSeconds(delay);
		netView.RPC("RPCRestart", RPCMode.All, mapName);
	}

	public bool IsRestarting()
	{
		return restarting;
	}

	public void RegisterHost()
	{
		string comment = config.server.country + "|" + config.server.name + "|" + config.server.versionCode + "|" + config.maps.current.path + "|" + config.maps.current.version;
		if (!Config.isLocal)
		{
			MasterServer.RegisterHost(config.server.type, config.maps.current.name, comment);
			return;
		}
		HostData hostData = new HostData();
		hostData.port = config.server.port;
		hostData.gameType = config.server.type;
		hostData.gameName = config.maps.current.name;
		hostData.comment = comment;
		hostData.passwordProtected = false;
		hostData.connectedPlayers = Network.connections.Length + 1;
		hostData.playerLimit = config.server.connections;
		networkDiscovery.Run(true, hostData);
	}

	public void OnChooseTeam(GameObject obj)
	{
		if (obj == null)
		{
			if (players.GetLocalPlayer().GetTeam() != Team.Spectator)
			{
				ui.Show(Ui.UiState.MenuInGame);
			}
			else
			{
				ui.MessageTopLong(Ui.MessageType.Info, "You need to choose a team !");
			}
			return;
		}
		Team team = (obj.name.StartsWith("CT") ? Team.Counter : Team.Terro);
		Debug.Log("OnChooseTeam: " + obj.name + " Team:" + team);
		if (players.GetLocalPlayer().GetTeam() == team)
		{
			ui.MessageTopLong(Ui.MessageType.Info, "You already are a " + team);
			return;
		}
		if (players.GetCount(team) >= 5)
		{
			ui.MessageTopLong(Ui.MessageType.Info, team.ToString() + " team is full !");
			return;
		}
		int num = (int)Enum.Parse(typeof(Model.ModelType), obj.name);
		Debug.Log("OnChooseTeam: " + obj.name + " modelType:" + num);
		netView.RPC("RPCSetTeam", RPCMode.All, players.GetLocalPlayer().GetNetworkPlayerID(), (int)team, num);
	}

	public void ChooseTeam(int terroCount, int counterCount)
	{
		if (players.GetLocalPlayer().GetTeam() != Team.Spectator)
		{
			terroCount = players.GetCount(Team.Terro);
			counterCount = players.GetCount(Team.Counter);
		}
		string text = ((counterCount <= terroCount) ? ("[00FF00]" + counterCount + " counters[-]") : ("[FF0000]" + counterCount + " counters[-]"));
		string text2 = ((terroCount <= counterCount) ? ("[00FF00]" + terroCount + " terros[-]") : ("[FF0000]" + terroCount + " terros[-]"));
		ui.Show(Ui.UiState.MenuChooseTeam, config.server.name + "\n\nThere is actually " + text + " and " + text2 + " playing on this server.\n\nChoose the right team :)");
	}

	public string GetRemainingTime()
	{
		int num = Mathf.CeilToInt(timeRemainingRound);
		if (num < 0)
		{
			return "0:00";
		}
		int num2 = num % 60;
		int num3 = num / 60;
		return string.Format("{0:00}:{1:00}", num3, num2);
	}

	public void OnNetworkLoadedLevel(string mapName)
	{
		Debug.Log("OnNetworkLoadedLevel: " + mapName);
		Screen.sleepTimeout = -1;
		music.GetComponent<AudioSource>().Stop();
		if (Network.isServer && !isInitialized)
		{
			Debug.Log("IM AM SERVER");
			config.maps.Set(mapName);
			Network.maxConnections = config.server.connections;
			if (!config.server.batchMode)
			{
				Network.maxConnections = config.server.connections - 1;
				NetworkViewID viewID = Network.AllocateViewID();
				Player.PlayerData data = new Player.PlayerData(Preferences.GetPlayerName(), config.server.country);
				AddPlayer(data, Network.player, viewID);
				ChooseTeam(0, 0);
			}
			isInitialized = true;
			RegisterHost();
		}
		if (Network.isClient && Network.connections.Length > 0)
		{
			Debug.Log("IM AM CLIENT");
			NetworkViewID networkViewID = Network.AllocateViewID();
			netView.RPC("RequestPlayer", RPCMode.Server, config.server.country, Preferences.GetPlayerName(), networkViewID);
		}
		startTimeMap = Time.time;
	}

	private void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		if (Network.isServer)
		{
			Debug.Log("OnPlayerConnected: player " + networkPlayer.ToString() + " connected");
			RegisterHost();
		}
	}

	private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
		if (!Network.isServer)
		{
			return;
		}
		Debug.Log("OnPlayerDisconnected: player " + networkPlayer.ToString() + " disconnected");
		Player player = players.GetPlayer(networkPlayer);
		if (player != null)
		{
			netView.RPC("RPCRemovePlayer", RPCMode.All, player.GetNetworkPlayerID());
			if (players.GetCount() <= 1 || players.GetAlive(Team.Counter) <= 0 || players.GetAlive(Team.Terro) <= 0)
			{
				netView.RPC("RPCRestart", RPCMode.All, "none");
			}
		}
		RegisterHost();
	}

	public void AddPlayer(Player.PlayerData data, NetworkPlayer networkPlayer, NetworkViewID viewID)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		gameObject.transform.parent = playersHolder;
		Player component = gameObject.GetComponent<Player>();
		component.game = this;
		component.Init(data, networkPlayer, viewID);
		players.Add(component);
	}

	public int GetScore(Team team)
	{
		return score[(int)team];
	}

	public void ShowLB()
	{
		cloud.ShowLeaderBoards();
	}

	public void QuitWithConfirm()
	{
		if (exiting)
		{
			return;
		}
		ui.box.Show(UIMessageBox.UIMessageType.Choice, "Quit", "Do you really want to quit ?", (UIMessageBox.UIAction action) =>
		{
			if (action == UIMessageBox.UIAction.Confirm)
			{
				Quit();
			}
			ui.box.Hide();
		});
	}

	public void Quit()
	{
		if (!exiting)
		{
			exiting = true;
			Network.Disconnect();
			cloud.Save();
			SceneManager.LoadScene(1);
		}
	}
}
