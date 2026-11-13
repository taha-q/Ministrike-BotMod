using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkInfo : MonoBehaviour
{
	public string MasterServerIpAddress;

	public int MasterServerPort;

	public string NetworkNatFacilitatorIP;

	public int NetworkNatFacilitatorPort;

	public bool forceNAT;

	public GameLogic game;

	public HostData server;

	public GameObject serverMenu;

	public GameObject startServerWindow;

	private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;

	private bool probingPublicIP;

	public bool doneTesting;

	private float timer;

	public bool useNat;

	public bool doneTestingLocation;

	public void Load()
	{
		MasterServer.ipAddress = MasterServerIpAddress;
		MasterServer.port = MasterServerPort;
		Network.natFacilitatorIP = NetworkNatFacilitatorIP;
		Network.natFacilitatorPort = NetworkNatFacilitatorPort;
		TestConnection();
		if (Network.HavePublicAddress())
		{
			Debug.Log("This machine has a public IP address");
		}
		else
		{
			Debug.Log("This machine has a private IP address");
		}
		game.location.UpdateLocation((bool success) =>
		{
			if (success)
			{
				Debug.Log("UpdateLocation success");
				game.config.server.country = game.location.country.ToLower();
			}
			else
			{
				Debug.LogError("UpdateLocation error");
			}
			doneTestingLocation = true;
		});
	}

	private void Update()
	{
		if (doneTestingLocation)
		{
			if (!doneTesting)
			{
				TestConnection();
			}
			else if (game.config.server.batchMode)
			{
				Debug.Log("InitializeServer( 10, " + game.config.server.port + ", " + useNat + " )");
				Network.InitializeServer(game.config.server.connections, game.config.server.port, useNat);
				MasterServer.dedicatedServer = true;
				game.OnNetworkLoadedLevel(game.config.maps.current.name);
				base.enabled = false;
			}
		}
	}

	private void TestConnection()
	{
		if (forceNAT)
		{
			useNat = true;
			doneTesting = true;
			return;
		}
		connectionTestResult = Network.TestConnection();
		switch (connectionTestResult)
		{
		case ConnectionTesterStatus.Error:
			doneTesting = true;
			Debug.LogError("Problem determining NAT capabilities");
			break;
		case ConnectionTesterStatus.Undetermined:
			doneTesting = false;
			break;
		case ConnectionTesterStatus.PublicIPIsConnectable:
			useNat = false;
			doneTesting = true;
			Debug.Log("Directly connectable public IP address");
			break;
		case ConnectionTesterStatus.PublicIPPortBlocked:
			useNat = false;
			if (!probingPublicIP)
			{
				Debug.Log("Testing if firewall can be circumvented");
				connectionTestResult = Network.TestConnectionNAT();
				probingPublicIP = true;
				timer = Time.time + 10f;
			}
			else if (Time.time > timer)
			{
				probingPublicIP = false;
				useNat = true;
				doneTesting = true;
				Debug.Log("probingPublicIP = false");
			}
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			Debug.Log("Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.");
			useNat = false;
			doneTesting = true;
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			Debug.Log("LimitedNATPunchthroughPortRestricted");
			useNat = true;
			doneTesting = true;
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			Debug.Log("Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is not adviced as not everyone can connect.");
			useNat = true;
			doneTesting = true;
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			Debug.Log("NAT punchthrough capable. Can connect to all servers andreceive connections from all clients. Enabling NAT punchthrough functionality.");
			useNat = true;
			doneTesting = true;
			break;
		default:
			Debug.LogError("Error in test routine, got " + connectionTestResult);
			break;
		}
	}

	private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		game.ui.MessageBottomLong(Ui.MessageType.Info, "Can't connect to master server !");
	}

	private void OnFailedToConnect(NetworkConnectionError info)
	{
		Debug.LogError(info);
		game.ui.MessageBottomLong(Ui.MessageType.Info, "Can't connect to server :\n" + info);
		if (info.ToString().Equals("InvalidPassword"))
		{
			game.ui.menuServers.gameObject.SetActive(true);
			game.ui.menuServers.GetComponent<UITweener>().PlayForward();
		}
		else
		{
			StartCoroutine(LoadServerListScene());
		}
	}

	private void OnDisconnectedFromServer()
	{
		game.ui.MessageBottomLong(Ui.MessageType.Info, "Disconnected from server !");
		StartCoroutine(LoadServerListScene());
	}

	private IEnumerator LoadTitleScene()
	{
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene(0);
	}

	private IEnumerator LoadServerListScene()
	{
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene(1);
	}
}
