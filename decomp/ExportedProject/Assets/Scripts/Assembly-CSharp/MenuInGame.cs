using System.Collections.Generic;
using UnityEngine;

public class MenuInGame : MonoBehaviour
{
	public GameLogic game;

	public List<UiStatItem> statsTerro;

	public List<UiStatItem> statsCounter;

	public UILabel terroScore;

	public UILabel counterScore;

	public UILabel nameLabel;

	public GameObject stats;

	public GameObject rcon;

	public GameObject controls;

	public GameObject leaderboards;

	public UIPopupList mapsPopupList;

	public UIPlayTween tweener;

	private void Start()
	{
		mapsPopupList.items = game.config.maps.GetNames();
		nameLabel.text = Preferences.GetPlayerName();
	}

	private void Update()
	{
		if (stats.activeSelf)
		{
			UpdateScore();
			UpdateTeam(GameLogic.Team.Terro);
			UpdateTeam(GameLogic.Team.Counter);
		}
	}

	private void UpdateScore()
	{
		terroScore.text = string.Empty + game.GetScore(GameLogic.Team.Terro);
		counterScore.text = string.Empty + game.GetScore(GameLogic.Team.Counter);
	}

	private void UpdateTeam(GameLogic.Team team)
	{
		List<Player> list = game.players.Get(team);
		List<UiStatItem> list2 = ((team != GameLogic.Team.Terro) ? statsCounter : statsTerro);
		for (int i = 0; i < list2.Count; i++)
		{
			if (list == null || list.Count == 0 || i >= list.Count)
			{
				list2[i].gameObject.SetActive(false);
				continue;
			}
			list2[i].gameObject.SetActive(true);
			list2[i].UpdateStatus(list[i]);
		}
	}

	public void ShowMenu(GameObject buttonGo)
	{
		stats.SetActive(buttonGo.GetHashCode() == stats.GetHashCode());
		rcon.SetActive(buttonGo.GetHashCode() == rcon.GetHashCode());
		controls.SetActive(buttonGo.GetHashCode() == controls.GetHashCode());
		leaderboards.SetActive(buttonGo.GetHashCode() == leaderboards.GetHashCode());
	}

	public void OnChangeMap()
	{
		Debug.Log("OnChangeMap");
		if (Network.isServer)
		{
			game.Restart(mapsPopupList.value);
		}
		else
		{
			game.ui.box.Show("Nope !", "You need to run a server to be able to change a map !");
		}
	}

	public void OnChangeName()
	{
		Debug.Log("OnChangeName");
		string text = nameLabel.text;
		if (text.Length < 4)
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "4 chars minimum");
			return;
		}
		if (text.Length > 15)
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "15 chars maximum");
			return;
		}
		game.netView.RPC("RPCSetName", RPCMode.All, game.players.GetLocalPlayer().GetNetworkPlayerID(), text);
	}
}
