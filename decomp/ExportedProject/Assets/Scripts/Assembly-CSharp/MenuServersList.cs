using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuServersList : MonoBehaviour
{
	public enum Filter
	{
		None = 0,
		Players = 1,
		Location = 2,
		Map = 3,
		Name = 4
	}

	public Filter filter;

	public GameLogic game;

	public NetworkInfo networkInfo;

	public UITweener tweenerMenuServer;

	public UITweener tweenerMenuServerCreate;

	public UITweener tweenerMenuServerPassword;

	public UILabel message;

	public UILabel gameComment;

	public UILabel gamePassword;

	public UILabel gamePasswordConnect;

	public GameObject serverItem;

	public UIGrid grid;

	public UIPopupList popupMapList;

	public UIPopupList popupCreateMapList;

	public UISprite pagePlus;

	public UISprite pageMinus;

	public UILabel pageLabel;

	public int page;

	public int count_per_page = 100;

	private List<HostData> serversList;

	private List<HostData> serversListFiltered;

	private bool updating;

	private bool sortPlayersAsc = true;

	private bool sortLocationAsc = true;

	private string filteredMap = "All";

	private void OnEnable()
	{
		Screen.sleepTimeout = -2;
	}

	private void OnDisable()
	{
		updating = false;
		message.text = string.Empty;
	}

	public void UpdateMapList()
	{
		popupMapList.items = game.config.maps.GetNames();
		popupMapList.items.Insert(0, "All");
		popupCreateMapList.items = game.config.maps.GetNames();
		popupCreateMapList.value = game.config.maps.GetNames()[0];
	}

	private IEnumerator UpdateServersItems()
	{
		updating = true;
		while (grid.transform.childCount > 0)
		{
			Object.DestroyImmediate(grid.transform.GetChild(0).gameObject);
		}
		if (serversListFiltered != null)
		{
			int num = page * count_per_page;
			int num2 = Mathf.Min(num + count_per_page, serversListFiltered.Count - 1);
			for (int i = num; i <= num2; i++)
			{
				GameObject gameObject = grid.gameObject.AddChild(serverItem);
				gameObject.GetComponent<UIServerItem>().SetHostData(serversListFiltered[i]);
				EventDelegate eventDelegate = new EventDelegate();
				EventDelegate.Parameter parameter = new EventDelegate.Parameter();
				parameter.obj = base.gameObject;
				parameter.value = gameObject;
				eventDelegate.Set(this, "OnConnectServerButton");
				eventDelegate.parameters.SetValue(parameter, 0);
				gameObject.GetComponent<UIEventTrigger>().onClick.Add(eventDelegate);
			}
			pageLabel.text = page + 1 + "/" + (serversListFiltered.Count / count_per_page + 1);
		}
		message.text = string.Empty;
		grid.gameObject.SetActive(true);
		grid.Reposition();
		grid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		updating = false;
		yield return null;
	}

	private void FilterServers(Filter f)
	{
		if (serversList == null)
		{
			return;
		}
		filter = f;
		switch (filter)
		{
		case Filter.Map:
			if (string.IsNullOrEmpty(filteredMap) || filteredMap.Equals("All"))
			{
				serversListFiltered = serversList;
				break;
			}
			serversListFiltered = serversList.FindAll((HostData data) => data.gameName.Contains(filteredMap));
			break;
		case Filter.Players:
			serversListFiltered = serversList;
			if (!sortPlayersAsc)
			{
				serversListFiltered.Sort((HostData x, HostData y) => x.connectedPlayers.CompareTo(y.connectedPlayers));
			}
			else
			{
				serversListFiltered.Sort((HostData x, HostData y) => y.connectedPlayers.CompareTo(x.connectedPlayers));
			}
			break;
		case Filter.Location:
			if (!sortLocationAsc)
			{
				serversListFiltered.Sort((HostData x, HostData y) => x.gameName.CompareTo(y.gameName));
			}
			else
			{
				serversListFiltered.Sort((HostData x, HostData y) => y.gameName.CompareTo(x.gameName));
			}
			break;
		case Filter.None:
			serversListFiltered = serversList;
			break;
		default:
			serversListFiltered = serversList;
			break;
		}
	}

	public void OnPagePlus()
	{
		if (serversListFiltered != null && page < serversListFiltered.Count / count_per_page)
		{
			page++;
			StartCoroutine(UpdateServersItems());
		}
	}

	public void OnPageMinus()
	{
		if (serversListFiltered != null && page > 0)
		{
			page--;
			StartCoroutine(UpdateServersItems());
		}
	}

	public void OnSortByPlayers()
	{
		if (!updating)
		{
			sortPlayersAsc = !sortPlayersAsc;
			FilterServers(Filter.Players);
			StartCoroutine(UpdateServersItems());
		}
	}

	public void OnSortByLocation()
	{
		if (!updating)
		{
			sortLocationAsc = !sortLocationAsc;
			FilterServers(Filter.Players);
			StartCoroutine(UpdateServersItems());
		}
	}

	public void OnSortByMapButton()
	{
		grid.gameObject.SetActive(false);
	}

	public void OnSortByMap(string mapName)
	{
		if (!updating)
		{
			filteredMap = mapName;
			FilterServers(Filter.Map);
			StartCoroutine(UpdateServersItems());
		}
	}

	private void OnConnectedToServer()
	{
		game.OnNetworkLoadedLevel(networkInfo.server.gameName);
	}

	private IEnumerator UpdateServersListLocal()
	{
		if (updating)
		{
			yield break;
		}
		updating = true;
		Debug.Log("UpdateServersListLocal");
		grid.gameObject.SetActive(false);
		while (grid.transform.childCount > 0)
		{
			Object.DestroyImmediate(grid.transform.GetChild(0).gameObject);
		}
		message.text = "Looking for local servers";
		game.networkDiscovery.Stop();
		game.networkDiscovery.Run(false, null);
		float startTime = Time.time;
		while (updating && game.networkDiscovery.clientHostData.port <= 0)
		{
			if (Time.time > startTime + 10f)
			{
				Debug.Log("Requesting servers list");
				startTime = Time.time;
			}
			string points = ((Random.Range(1, 3) != 1) ? ".." : ".");
			message.text = "Updating servers list\n" + points;
			yield return null;
		}
		game.networkDiscovery.Stop();
		if (updating)
		{
			Debug.Log("HostList received");
			serversList = new List<HostData>();
			serversList.Add(game.networkDiscovery.clientHostData);
			FilterServers(Filter.Players);
			yield return StartCoroutine(UpdateServersItems());
		}
		message.text = string.Empty;
		updating = false;
		yield return null;
	}

	private IEnumerator UpdateServersList()
	{
		if (updating)
		{
			yield break;
		}
		Debug.Log("UpdateServersList");
		updating = true;
		grid.gameObject.SetActive(false);
		while (grid.transform.childCount > 0)
		{
			Object.DestroyImmediate(grid.transform.GetChild(0).gameObject);
		}
		MasterServer.ClearHostList();
		Debug.Log("Requesting servers list");
		MasterServer.RequestHostList(game.config.server.type);
		float startTime = Time.time;
		while (MasterServer.PollHostList().Length == 0 && updating)
		{
			if (Time.time > startTime + 10f)
			{
				Debug.Log("Requesting servers list");
				MasterServer.RequestHostList(game.config.server.type);
				startTime = Time.time;
			}
			string points = ((Random.Range(1, 3) != 1) ? ".." : ".");
			message.text = "Updating servers list\n" + points;
			yield return null;
		}
		if (!updating)
		{
			message.text = string.Empty;
			yield break;
		}
		Debug.Log("HostList received");
		HostData[] servers = MasterServer.PollHostList();
		serversList = new List<HostData>(servers);
		FilterServers(Filter.Players);
		yield return StartCoroutine(UpdateServersItems());
		message.text = string.Empty;
		updating = false;
	}

	public void OnConnectServerButton(GameObject obj)
	{
		UIServerItem component = obj.GetComponent<UIServerItem>();
		if (component.versionCode != game.config.server.versionCode)
		{
			game.ui.box.Show(UIMessageBox.UIMessageType.Info, "Oups", "Your client version (" + game.config.server.versionCode + ") is not compatible with this server version (" + component.versionCode + ")\n\nPlease update your client (or server)");
			return;
		}
		Config.Map map = game.config.maps.Get(component.GetMap());
		if (map == null)
		{
			Debug.Log("Map not available: " + component.GetMap() + ", downloading...");
			map = new Config.Map();
			map.name = component.GetMap();
			map.path = component.mapPath;
			map.version = component.mapVersion;
			Debug.Log("map.name=" + map.name + " - map.path=" + map.path + " - map.version=" + map.version);
			game.ui.box.Show(UIMessageBox.UIMessageType.Infiny, "Please Wait", "Loading configuration and maps...");
			game.config.DownloadMap(map, (bool success, string message, int progress) =>
			{
				if (progress != 100)
				{
					game.ui.box.SetTextLabel(string.Concat("Loading configuration and maps...\n\nDownloading map: ", map, " (", progress, "/100)"));
				}
				else
				{
					game.ui.box.Hide();
					if (success)
					{
						OnConnectServerButton(obj);
					}
					else
					{
						game.ui.box.Show(UIMessageBox.UIMessageType.Info, "Error", "Could not download map:\n" + message);
					}
				}
			});
			return;
		}
		networkInfo.server = component.GetHostData();
		if (networkInfo.server.passwordProtected)
		{
			tweenerMenuServerPassword.gameObject.SetActive(true);
			tweenerMenuServer.PlayReverse();
			tweenerMenuServerPassword.PlayForward();
			return;
		}
		Debug.Log("connecting to server: " + networkInfo.server.ip[0]);
		if (Config.isLocal)
		{
			Network.Connect(networkInfo.server.ip[0], networkInfo.server.port);
		}
		else
		{
			Network.Connect(networkInfo.server);
		}
		tweenerMenuServer.PlayReverse();
	}

	public void OnConnectServerButtonWithPassword()
	{
		Debug.Log("connecting to server: " + networkInfo.server.ip[0]);
		Network.Connect(networkInfo.server, gamePasswordConnect.text);
		tweenerMenuServer.PlayReverse();
	}

	public void OnUpdateServers()
	{
		if (!updating)
		{
			if (Config.isLocal)
			{
				StartCoroutine(UpdateServersListLocal());
			}
			else
			{
				StartCoroutine(UpdateServersList());
			}
		}
	}

	public void CreateServer()
	{
		if (Config.isLocal)
		{
			StartServerLocal();
		}
		else
		{
			StartServer();
		}
	}

	private void StartServer()
	{
		Debug.Log("StartServer");
		if (Network.peerType == NetworkPeerType.Disconnected && networkInfo.doneTesting)
		{
			tweenerMenuServerCreate.Play(false);
			game.config.server.name = gameComment.text;
			Network.incomingPassword = gamePassword.text;
			Debug.Log("InitializeServer( " + game.config.server.connections + ", " + game.config.server.port + ", " + networkInfo.useNat + " )");
			Network.InitializeServer(game.config.server.connections, game.config.server.port, networkInfo.useNat);
			game.OnNetworkLoadedLevel(game.config.maps.current.name);
		}
		else
		{
			game.ui.MessageBottomShort(Ui.MessageType.Info, "Please wait for network initialization");
		}
	}

	private void StartServerLocal()
	{
		Debug.Log("StartServerLocal");
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			tweenerMenuServerCreate.Play(false);
			game.config.server.name = gameComment.text;
			Network.incomingPassword = gamePassword.text;
			Debug.Log("InitializeServer( " + game.config.server.connections + ", " + game.config.server.port + ", " + networkInfo.useNat + " )");
			Network.InitializeServer(game.config.server.connections, game.config.server.port, networkInfo.useNat);
			game.OnNetworkLoadedLevel(game.config.maps.current.name);
		}
	}
}
