using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
	internal struct State
	{
		internal double timestamp;

		internal Vector3 pos;
	}

	public int damage = 200;

	public float speed = 30f;

	public GameObject explosion;

	public double interpolationBackTime = 0.1;

	private Player player;

	private NetworkView netView;

	private Transform thisTrans;

	private bool exploding;

	private State[] m_BufferedState = new State[20];

	private int m_TimestampCount;

	private void Start()
	{
		thisTrans = base.transform;
		netView = GetComponent<NetworkView>();
	}

	public void SetPlayer(Player p)
	{
		player = p;
	}

	public void InitExplosion()
	{
		StartCoroutine(ExplodeRoutine());
	}

	private IEnumerator ExplodeRoutine()
	{
		yield return new WaitForSeconds(1.5f);
		Explode();
	}

	public void Explode()
	{
		Explode(player);
	}

	public void Explode(Player playerSource)
	{
		if (exploding)
		{
			return;
		}
		exploding = true;
		Vector3 position = thisTrans.position;
		position.y += 0.5f;
		Object.Instantiate(explosion, position, Quaternion.identity);
		if (!playerSource)
		{
			return;
		}
		if (Network.isServer)
		{
			List<Player> list = playerSource.game.players.Get();
			foreach (Player item in list)
			{
				float num = Vector3.Distance(thisTrans.position, item.transform.position);
				if (num < 8f)
				{
					damage /= Mathf.Max((int)num, 1);
					playerSource.game.netView.RPC("RPCHit", RPCMode.All, playerSource.GetNetworkPlayerID(), item.GetNetworkPlayerID(), damage);
				}
			}
		}
		Object.Destroy(base.gameObject);
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 value = thisTrans.position;
			stream.Serialize(ref value);
			return;
		}
		Vector3 value2 = Vector3.zero;
		stream.Serialize(ref value2);
		for (int num = m_BufferedState.Length - 1; num >= 1; num--)
		{
			m_BufferedState[num] = m_BufferedState[num - 1];
		}
		State state = default(State);
		state.timestamp = info.timestamp;
		state.pos = value2;
		m_BufferedState[0] = state;
		m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);
	}

	private void Update()
	{
		if (netView.isMine)
		{
			return;
		}
		double time = Network.time;
		double num = time - interpolationBackTime;
		if (m_BufferedState[0].timestamp > num)
		{
			for (int i = 0; i < m_TimestampCount; i++)
			{
				if (m_BufferedState[i].timestamp <= num || i == m_TimestampCount - 1)
				{
					State state = m_BufferedState[Mathf.Max(i - 1, 0)];
					State state2 = m_BufferedState[i];
					double num2 = state.timestamp - state2.timestamp;
					float t = 0f;
					if (num2 > 0.0001)
					{
						t = (float)((num - state2.timestamp) / num2);
					}
					thisTrans.position = Vector3.Lerp(state2.pos, state.pos, t);
					break;
				}
			}
		}
		else
		{
			State state3 = m_BufferedState[0];
			thisTrans.position = state3.pos;
		}
	}
}
