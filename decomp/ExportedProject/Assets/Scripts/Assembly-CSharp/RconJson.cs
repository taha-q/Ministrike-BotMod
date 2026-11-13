using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class RconJson : MonoBehaviour
{
	public GameLogic game;

	private Thread rconThread;

	private bool stop;

	private void Start()
	{
		rconThread = new Thread(rconThreadRoutine);
		rconThread.Start();
	}

	private void rconThreadRoutine()
	{
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, 4321);
		TcpListener tcpListener = new TcpListener(iPEndPoint);
		tcpListener.Start();
		Debug.Log(string.Concat("Started listening requests at: ", iPEndPoint.Address, ":", iPEndPoint.Port));
		while (!stop)
		{
			byte[] buffer = new byte[1048576];
			TcpClient tcpClient = tcpListener.AcceptTcpClient();
			tcpClient.GetStream().Read(buffer, 0, 1048576);
			JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject.AddField("name", game.config.server.name);
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
			jSONObject.AddField("players", jSONObject2);
			foreach (Player item in game.players.Get())
			{
				jSONObject2.AddField("name", item.name);
			}
			byte[] bytes = Encoding.Unicode.GetBytes(jSONObject.Print());
			tcpClient.GetStream().Write(bytes, 0, bytes.Length);
			tcpClient.Close();
		}
	}

	private static string cleanMessage(byte[] bytes)
	{
		string text = Encoding.Unicode.GetString(bytes);
		string text2 = null;
		string text3 = text;
		foreach (char c in text3)
		{
			if (c != 0)
			{
				text2 += c;
			}
		}
		return text2;
	}

	private static void sendMessage(byte[] bytes, TcpClient client)
	{
		client.GetStream().Write(bytes, 0, bytes.Length);
	}

	private void OnDestroy()
	{
		stop = true;
		rconThread.Abort();
	}

	private void OnDisable()
	{
		stop = true;
	}

	private void OnApplicationQuit()
	{
		stop = true;
	}
}
