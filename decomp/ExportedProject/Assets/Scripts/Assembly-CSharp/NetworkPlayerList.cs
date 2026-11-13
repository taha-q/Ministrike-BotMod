using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerList : MonoBehaviour
{
	private List<Player> players = new List<Player>();

	private int randomPlayer;

	public void Add(Player player)
	{
		players.Add(player);
	}

	public void Remove(Player player)
	{
		players.Remove(player);
	}

	public List<Player> Get()
	{
		return players;
	}

	public List<Player> Get(GameLogic.Team team)
	{
		List<Player> list = players.FindAll((Player p) => p.GetTeam() == team);
		if (list != null && list.Count > 1)
		{
			list.Sort((Player x, Player y) => y.GetKills().CompareTo(x.GetKills()));
		}
		return list;
	}

	public Player GetByViewID(NetworkViewID viewID)
	{
		return players.Find((Player p) => p.GetNetworkViewID() == viewID);
	}

	public Player GetByPlayerID(int networkPlayerID)
	{
		return players.Find((Player p) => p.GetNetworkPlayerID() == networkPlayerID);
	}

	public Player GetPlayer(NetworkPlayer networkPlayer)
	{
		return players.Find((Player p) => p.GetNetworkPlayer() == networkPlayer);
	}

	public Player GetPlayerRandom()
	{
		if (randomPlayer >= players.Count)
		{
			randomPlayer = 0;
		}
		for (int i = randomPlayer; i < players.Count; i++)
		{
			randomPlayer++;
			if (!players[i].isMine && players[i].GetLife() > 0)
			{
				return players[i];
			}
		}
		randomPlayer = 0;
		for (int j = 0; j < players.Count; j++)
		{
			randomPlayer++;
			if (!players[j].isMine && players[j].GetLife() > 0)
			{
				return players[j];
			}
		}
		return null;
	}

	public Player GetLocalPlayer()
	{
		return players.Find((Player p) => p.isMine);
	}

	public int GetAlive(GameLogic.Team team)
	{
		List<Player> list = players.FindAll((Player p) => p.GetLife() > 0 && p.GetTeam() == team);
		if (list != null)
		{
			return list.Count;
		}
		return 0;
	}

	public int GetCount(GameLogic.Team team)
	{
		List<Player> list = players.FindAll((Player p) => p.GetTeam() == team);
		if (list != null)
		{
			return list.Count;
		}
		return 0;
	}

	public int GetCount()
	{
		return players.Count;
	}

	public bool IsNameAvailable(string name)
	{
		return !players.Exists((Player p) => p.name == name);
	}
}
