using UnityEngine;

public class NetworkInterpolatedTransform : MonoBehaviour
{
	internal struct State
	{
		internal double timestamp;

		internal Vector3 pos;

		internal Quaternion rot;
	}

	public Player player;

	public double interpolationBackTime = 0.1;

	private State[] m_BufferedState = new State[20];

	private int m_TimestampCount;

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (player.GetLife() > 0 && stream.isWriting)
		{
			Vector3 value = base.transform.position;
			Quaternion value2 = base.transform.localRotation;
			stream.Serialize(ref value);
			stream.Serialize(ref value2);
			Vector3 value3 = player.animationController.hit.point;
			stream.Serialize(ref value3);
			Vector3 value4 = player.animationController.move;
			stream.Serialize(ref value4);
		}
		else if (player.GetLife() > 0 && stream.isReading)
		{
			Vector3 value5 = Vector3.zero;
			Quaternion value6 = Quaternion.identity;
			stream.Serialize(ref value5);
			stream.Serialize(ref value6);
			Vector3 value7 = Vector3.zero;
			stream.Serialize(ref value7);
			player.animationController.hit.point = value7;
			Vector3 value8 = Vector3.zero;
			stream.Serialize(ref value8);
			player.animationController.OnMove(value8);
			for (int num = m_BufferedState.Length - 1; num >= 1; num--)
			{
				m_BufferedState[num] = m_BufferedState[num - 1];
			}
			State state = default(State);
			state.timestamp = info.timestamp;
			state.pos = value5;
			state.rot = value6;
			m_BufferedState[0] = state;
			m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);
		}
	}

	private void Update()
	{
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
					base.transform.position = Vector3.Lerp(state2.pos, state.pos, t);
					base.transform.localRotation = Quaternion.Lerp(state2.rot, state.rot, t);
					break;
				}
			}
		}
		else
		{
			State state3 = m_BufferedState[0];
			base.transform.position = state3.pos;
			base.transform.localRotation = state3.rot;
		}
	}
}
