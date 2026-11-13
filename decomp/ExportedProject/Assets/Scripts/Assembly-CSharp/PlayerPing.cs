using UnityEngine;

public class PlayerPing : MonoBehaviour
{
	private Player player;

	private GameLogic game;

	private float time = 15f;

	private bool alive = true;

	private void Start()
	{
		player = GetComponent<Player>();
		game = player.game;
		time = 15f;
	}

	private void Update()
	{
		if (!Network.isServer || player.isMine)
		{
			return;
		}
		time -= Time.deltaTime;
		if (time <= 0f && alive)
		{
			alive = false;
			player.netView.RPC("PingPlayer", player.GetNetworkPlayer());
		}
		else if (time < -15f && !alive)
		{
			Debug.LogError(player.name + " didn't respond to ping request, forcing disconnect !");
			game.netView.RPC("RPCRemovePlayer", RPCMode.All, player.GetNetworkPlayerID());
			if (game.players.GetCount() <= 1 || game.players.GetAlive(GameLogic.Team.Counter) <= 0 || game.players.GetAlive(GameLogic.Team.Terro) <= 0)
			{
				game.netView.RPC("RPCRestart", RPCMode.All, "none");
			}
		}
	}

	[RPC]
	private void PingPlayer()
	{
		player.netView.RPC("Reply", RPCMode.Server);
	}

	[RPC]
	private void Reply()
	{
		player.SetPing((int)(Mathf.Abs(time) * 1000f));
		time = 15f;
		alive = true;
	}
}
