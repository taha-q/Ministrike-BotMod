using System;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDiscoveryEx : NetworkDisco
{
	[Serializable]
	private class ServerInfo
	{
		public int port;

		public int connectedPlayers;

		public int playerLimit;

		public string gameType;

		public string gameName;

		public string comment;
	}

	public HostData clientHostData;

	public void Run(bool _isServer, HostData hostData)
	{
		Stop();
		base.isServer = _isServer;
		base.isClient = !base.isServer;
		Debug.Log("NetworkDiscoveryEx.Init: isServer = " + base.isServer);
		if (base.isServer)
		{
			ServerInfo serverInfo = new ServerInfo();
			serverInfo.port = hostData.port;
			serverInfo.gameType = hostData.gameType;
			serverInfo.gameName = hostData.gameName;
			serverInfo.comment = hostData.comment;
			serverInfo.connectedPlayers = hostData.connectedPlayers;
			serverInfo.playerLimit = hostData.playerLimit;
			string text = Utility.serialize(serverInfo);
			base.broadcastData = text;
			Debug.Log("NetworkDiscoveryEx: broadcastData: len = " + base.broadcastData.Length + " data = " + base.broadcastData);
		}
		else
		{
			clientHostData = new HostData();
			clientHostData.port = 0;
		}
		Initialize();
	}

	public void Stop()
	{
		StopBroadcast();
		if (NetworkTransport.IsStarted && NetworkTransport.IsBroadcastDiscoveryRunning())
		{
			NetworkTransport.StopBroadcastDiscovery();
		}
	}

	public override void OnReceivedBroadcast(string fromAddress, string data)
	{
		string text = data.Trim();
		Debug.Log("NetworkDiscoveryEx.OnReceivedBroadcast: address = " + fromAddress + " dataLen = " + text.Length + " data = " + text);
		if (text == null)
		{
			return;
		}
		ServerInfo serverInfo = (ServerInfo)Utility.unserialize(text);
		if (serverInfo != null)
		{
			try
			{
				clientHostData.ip = new string[1] { fromAddress.Split(':')[3] };
			}
			catch (Exception)
			{
			}
			clientHostData.port = serverInfo.port;
			clientHostData.gameType = serverInfo.gameType;
			clientHostData.gameName = serverInfo.gameName;
			clientHostData.comment = serverInfo.comment;
			clientHostData.passwordProtected = false;
			clientHostData.playerLimit = serverInfo.playerLimit;
			clientHostData.connectedPlayers = serverInfo.connectedPlayers;
			Stop();
		}
	}
}
