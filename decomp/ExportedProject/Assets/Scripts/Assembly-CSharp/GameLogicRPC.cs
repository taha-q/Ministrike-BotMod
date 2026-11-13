using UnityEngine;

public class GameLogicRPC : MonoBehaviour
{
	private GameLogic gameLogic;

	private Config config;

	private NetworkPlayerList players;

	private NetworkView netView;

	private Ui ui;

	private void Start()
	{
		gameLogic = GetComponent<GameLogic>();
		netView = gameLogic.netView;
		config = gameLogic.config;
		players = gameLogic.players;
		ui = gameLogic.ui;
	}

	[RPC]
	private void RPCDisable(string name)
	{
		Transform transform = config.maps.current.transform.FindChild("Pickable");
		if (transform != null)
		{
			Transform transform2 = transform.FindChild(name);
			if (transform2 != null)
			{
				transform2.gameObject.SetActive(false);
			}
		}
	}

	[RPC]
	public void RPCRcon(string pwd, string arg0, string arg1)
	{
		if (!Network.isServer || string.IsNullOrEmpty(pwd) || pwd.Equals("none") || !pwd.Equals(config.server.password))
		{
			return;
		}
		switch (arg0)
		{
		case "map":
			gameLogic.Restart(arg1);
			break;
		case "restart":
			gameLogic.Restart();
			break;
		case "kick":
		{
			Player player = players.Get().Find((Player pl) => pl.name == arg1);
			if ((bool)player)
			{
				Debug.Log("RPCRcon.kick: " + player.name);
				Network.CloseConnection(player.networkPlayer, true);
			}
			break;
		}
		}
	}

	[RPC]
	private void RPCMessage(int networkPlayerID, string n)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			ui.MessageTopLong(Ui.MessageType.Chat, "[" + byPlayerID.name + "]: " + n);
		}
	}

	[RPC]
	private void RPCMessageInfo(string n)
	{
		ui.MessageTopLong(Ui.MessageType.Info, n);
	}

	[RPC]
	private void RPCSetName(int networkPlayerID, string n)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID == null)
		{
			Debug.LogError("RPCSetName: player == null");
		}
		else if (players.IsNameAvailable(n))
		{
			byPlayerID.SetName(n);
			if (byPlayerID.isMine)
			{
				ui.MessageTopLong(Ui.MessageType.Info, "Name changed to " + byPlayerID.name);
			}
		}
		else if (byPlayerID.isMine)
		{
			ui.MessageTopLong(Ui.MessageType.Info, "Name already in use !");
		}
	}

	[RPC]
	private void RPCRemovePlayer(int networkPlayerID)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			if (Network.isServer)
			{
				NetworkPlayer networkPlayer = byPlayerID.GetNetworkPlayer();
				Network.RemoveRPCs(networkPlayer);
				Network.DestroyPlayerObjects(networkPlayer);
			}
			Object.Destroy(byPlayerID.gameObject);
			players.Remove(byPlayerID);
		}
		else
		{
			Debug.LogError("RPCRemovePlayer: player == null");
		}
	}

	[RPC]
	private void RPCSetTeam(int networkPlayerID, int team, int modelType)
	{
		Debug.Log("RPCSetTeam " + networkPlayerID);
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			if (byPlayerID.GetLife() > 0)
			{
				Debug.Log("RPCSetTeam: player is alive, self killing");
				byPlayerID.Die(true, true);
			}
			byPlayerID.SetTeam((GameLogic.Team)team, modelType);
			byPlayerID.Reset(false, false);
			if (Network.isServer && (players.GetCount() <= 1 || players.GetAlive(GameLogic.Team.Counter) <= 0 || players.GetAlive(GameLogic.Team.Terro) <= 0))
			{
				netView.RPC("RPCRestart", RPCMode.All, "none");
			}
		}
		else
		{
			Debug.LogError("RPCSetTeam: player == null");
		}
	}

	[RPC]
	private void RPCDie(int fromNetworkPlayerID, int toNetworkPlayerID)
	{
		Player byPlayerID = players.GetByPlayerID(fromNetworkPlayerID);
		Player byPlayerID2 = players.GetByPlayerID(toNetworkPlayerID);
		if (byPlayerID != null && byPlayerID2 != null)
		{
			Debug.Log("RPCDie: " + byPlayerID.name + " killed " + byPlayerID2.name);
			if (fromNetworkPlayerID == toNetworkPlayerID)
			{
				ui.MessageTopLong(Ui.MessageType.Kill, byPlayerID.name + " killed himself, boooh !");
			}
			else
			{
				ui.MessageTopLong(Ui.MessageType.Kill, byPlayerID.name + " killed " + byPlayerID2.name + " !");
			}
			if (fromNetworkPlayerID != toNetworkPlayerID && byPlayerID2.GetTeam() != byPlayerID.GetTeam())
			{
				byPlayerID.AddKills();
			}
			byPlayerID2.Die(true);
			Player localPlayer = players.GetLocalPlayer();
			if (localPlayer != null && localPlayer.GetLife() <= 0)
			{
				Player targetPlayer = localPlayer.controller.cameraController.GetTargetPlayer();
				if (targetPlayer.GetNetworkPlayerID() == byPlayerID2.GetNetworkPlayerID())
				{
					localPlayer.SetCameraTarget((byPlayerID.GetLife() <= 0) ? null : byPlayerID);
				}
			}
		}
		else
		{
			Debug.LogError("RPCDie: fromPlayer or toPlayer == null");
		}
	}

	[RPC]
	private void RPCShootGrenade(int networkPlayerID, NetworkViewID viewID)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			byPlayerID.ShootGrenade(viewID);
		}
		else
		{
			Debug.LogError("RPCShootGrenade: player == null");
		}
	}

	[RPC]
	private void RPCAnim(int networkPlayerID, int animID)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			byPlayerID.animationController.Play((AnimationController.AnimState)animID);
			Player localPlayer = players.GetLocalPlayer();
			if (localPlayer != null)
			{
				switch (animID)
				{
				case 13:
					ui.MessageTopShort(Ui.MessageType.Chat, byPlayerID.name + ": you f*ck my wife ?!");
					break;
				case 14:
					ui.MessageTopShort(Ui.MessageType.Chat, byPlayerID.name + ": Yippee ki-yay !");
					break;
				}
			}
		}
		else
		{
			Debug.LogError("RPCAnim: player == null");
		}
	}

	[RPC]
	private void RPCShoot(int networkPlayerID, Vector3 hitPoint)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			byPlayerID.Shoot(hitPoint);
		}
		else
		{
			Debug.LogError("RPCShoot: player == null");
		}
	}

	[RPC]
	private void RPCSetSlot(int networkPlayerID, int slot)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			byPlayerID.GetInventory().SetSlot((Inventory.Slot)slot);
		}
		else
		{
			Debug.LogError("RPCSetSlot: player == null");
		}
	}

	[RPC]
	private void RPCRemoveWeapon(int networkPlayerID, string weaponName)
	{
		Debug.Log("RPCRemoveWeapon " + networkPlayerID + ": " + weaponName);
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			byPlayerID.GetInventory().RemoveWeapon(weaponName);
		}
		else
		{
			Debug.LogError("RPCRemoveWeapon: player == null");
		}
	}

	[RPC]
	private void RPCDropWeapon(int networkPlayerID, int bullets, int chargers)
	{
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			Debug.Log("RPCDropWeapon " + byPlayerID.name);
			Weapon weapon = byPlayerID.GetInventory().GetWeapon();
			if (weapon != null)
			{
				weapon.SetBullets(bullets);
				weapon.SetChargers(chargers);
				byPlayerID.GetInventory().DropWeapon();
			}
			else
			{
				Debug.Log("RPCDropWeapon: GetWeapon == null");
			}
		}
		else
		{
			Debug.LogError("RPCDropWeapon: player == null");
		}
	}

	[RPC]
	private void RPCAddWeapon(int networkPlayerID, string weaponName)
	{
		Debug.Log("RPCAddWeapon " + networkPlayerID + " - " + weaponName);
		Player byPlayerID = players.GetByPlayerID(networkPlayerID);
		if (byPlayerID != null)
		{
			byPlayerID.GetInventory().AddWeapon(weaponName);
		}
		else
		{
			Debug.LogError("RPCAddWeapon: player == null");
		}
	}

	[RPC]
	private void RPCHit(int fromNetworkPlayerID, int toNetworkPlayerID, int damage)
	{
		Player byPlayerID = players.GetByPlayerID(fromNetworkPlayerID);
		Player byPlayerID2 = players.GetByPlayerID(toNetworkPlayerID);
		if (byPlayerID2 == null || byPlayerID == null)
		{
			Debug.LogError("RPCHit: toPlayer or fromPlayer == null");
		}
		else
		{
			if (byPlayerID2.GetLife() <= 0)
			{
				return;
			}
			int num = byPlayerID2.GetShield() + byPlayerID2.GetLife() - damage;
			int b = num - 100;
			byPlayerID2.SetShield(Mathf.Max(0, b));
			byPlayerID2.SetLife(num - byPlayerID2.GetShield());
			if (byPlayerID2.isMine)
			{
				gameLogic.bloodTweener.Play(true);
				ui.hud.UpdateLife();
			}
			if (!Network.isServer || byPlayerID2.GetLife() > 0)
			{
				return;
			}
			RPCDie(byPlayerID.GetNetworkPlayerID(), byPlayerID2.GetNetworkPlayerID());
			netView.RPC("RPCDie", RPCMode.Others, byPlayerID.GetNetworkPlayerID(), byPlayerID2.GetNetworkPlayerID());
			int alive = players.GetAlive(byPlayerID2.GetTeam());
			if (alive <= 0)
			{
				Debug.Log("Round end");
				int team = (int)byPlayerID.GetTeam();
				if (byPlayerID2.GetTeam() != byPlayerID.GetTeam())
				{
					gameLogic.score[team]++;
					netView.RPC("RPCSetScore", RPCMode.Others, team, gameLogic.score[team]);
					ui.MessageTopLong(Ui.MessageType.Info, string.Concat((GameLogic.Team)team, " wins !"));
				}
				gameLogic.Restart();
			}
		}
	}

	[RPC]
	private void RPCSetScore(int team, int newScore)
	{
		gameLogic.score[team] = newScore;
		ui.MessageTopLong(Ui.MessageType.Info, string.Concat((GameLogic.Team)team, " wins !"));
	}

	[RPC]
	public void RPCRestart(string mapName)
	{
		Debug.Log("RPCRestart");
		gameLogic.cloud.SaveLB();
		bool flag = !mapName.Equals("none");
		if (flag)
		{
			gameLogic.startTimeMap = Time.time;
			config.maps.Set(mapName);
			gameLogic.score[1] = 0;
			gameLogic.score[2] = 0;
		}
		if (Network.isServer)
		{
			UpdatePlayers();
		}
		int alive = players.GetAlive(GameLogic.Team.Terro);
		int alive2 = players.GetAlive(GameLogic.Team.Counter);
		foreach (Player item in players.Get())
		{
			if (item.GetTeam() == GameLogic.Team.Spectator)
			{
				continue;
			}
			if (flag)
			{
				item.SetKills(0);
				item.SetDeaths(0);
				item.Reset(true, true);
			}
			else
			{
				item.Reset(true, false, item.GetLife() <= 0);
				if (item.GetTeam() == GameLogic.Team.Terro && alive > 0 && alive > alive2)
				{
					item.GetInventory().SetMoney(item.GetInventory().GetMoney() + 3000);
				}
				else if (item.GetTeam() == GameLogic.Team.Terro)
				{
					item.GetInventory().SetMoney(item.GetInventory().GetMoney() + 1500);
				}
				else if (item.GetTeam() == GameLogic.Team.Counter && alive2 > 0 && alive2 > alive)
				{
					item.GetInventory().SetMoney(item.GetInventory().GetMoney() + 3000);
				}
				else if (item.GetTeam() == GameLogic.Team.Counter)
				{
					item.GetInventory().SetMoney(item.GetInventory().GetMoney() + 1500);
				}
			}
			if (item.isMine)
			{
				ui.Show(Ui.UiState.Hud);
			}
		}
		for (int num = gameLogic.droppedWeapons.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(gameLogic.droppedWeapons.GetChild(num).gameObject);
		}
		Explodable[] array = Object.FindObjectsOfType<Explodable>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
		config.maps.Reset(config.maps.current);
		gameLogic.startTimeRound = Time.time;
		gameLogic.restarting = false;
	}

	[RPC]
	private void ReceivePlayer(string serializedData, NetworkPlayer networkPlayer, NetworkViewID viewID)
	{
		Player.PlayerData playerData = (Player.PlayerData)Utility.unserialize(serializedData);
		Player player = players.GetPlayer(networkPlayer);
		if (player == null)
		{
			Debug.Log("ReceivePlayer: adding player: \n" + playerData.ToString());
			gameLogic.AddPlayer(playerData, networkPlayer, viewID);
			Player localPlayer = players.GetLocalPlayer();
			if (localPlayer != null)
			{
				Player targetPlayer = localPlayer.controller.cameraController.GetTargetPlayer();
				if (targetPlayer == null)
				{
					localPlayer.SetCameraTarget(null);
				}
			}
		}
		else
		{
			Debug.Log("ReceivePlayer: updating player: \n" + playerData.ToString());
			player.Init(playerData, networkPlayer, viewID);
		}
	}

	[RPC]
	private void InitializeClient(string mapName, int terros, int counters, string name, NetworkPlayer networkPlayer, NetworkViewID viewID)
	{
		config.maps.Set(mapName);
		Player.PlayerData data = new Player.PlayerData(name, config.server.country);
		Debug.Log("InitializeClient: " + name);
		gameLogic.AddPlayer(data, networkPlayer, viewID);
		gameLogic.ChooseTeam(terros, counters);
		gameLogic.isInitialized = true;
		netView.RPC("UpdatePlayers", RPCMode.Server);
	}

	[RPC]
	private void UpdatePlayers()
	{
		if (!Network.isServer)
		{
			return;
		}
		foreach (Player item in players.Get())
		{
			foreach (Player item2 in players.Get())
			{
				if (item.GetNetworkPlayerID() != 0)
				{
					item2.data.inventory = item2.GetInventory().GetData();
					string text = Utility.serialize(item2.data);
					netView.RPC("ReceivePlayer", item.networkPlayer, text, item2.networkPlayer, item2.GetNetworkViewID());
					Debug.Log("UpdatePlayers: sending player info to \"" + item.name + "\" for \"" + item2.name + "\"");
				}
			}
		}
	}

	[RPC]
	private void RequestPlayer(string country, string name, NetworkViewID viewID, NetworkMessageInfo info)
	{
		string playerName = ((!players.IsNameAvailable(name)) ? (name + "-" + info.sender.ToString()) : name);
		Player.PlayerData playerData = new Player.PlayerData(playerName, country);
		gameLogic.AddPlayer(playerData, info.sender, viewID);
		Debug.Log("RequestPlayer: " + name);
		netView.RPC("InitializeClient", info.sender, config.maps.current.name, players.GetCount(GameLogic.Team.Terro), players.GetCount(GameLogic.Team.Counter), playerData.name, info.sender, viewID);
	}
}
