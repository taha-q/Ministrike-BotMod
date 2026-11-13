using System;
using System.Collections;
using System.Collections.Generic;
using SharpConfig;
using UnityEngine;

public class Config : MonoBehaviour
{
	[Serializable]
	public class Server
	{
		public string version = "1.0";

		public int versionCode;

		public string type = "MiniStrike";

		public string country = "??";

		public bool batchMode;

		public string name = "Welcome To The Party";

		public int timePerMap = 1200;

		public int timePerRound = 180;

		public int port = 25000;

		public int connections = 10;

		public string password = "none";
	}

	public class Maps
	{
		public Map current;

		private List<Map> list;

		public Maps()
		{
			list = new List<Map>();
		}

		public void Add(Map map)
		{
			list.Add(map);
		}

		public List<Map> Get()
		{
			return list;
		}

		public Map Get(string mapName)
		{
			return list.Find((Map m) => m.name.Equals(mapName));
		}

		public Map GetNext()
		{
			int num = list.FindIndex((Map m) => m.name == current.name) + 1;
			return list[(num < list.Count) ? num : 0];
		}

		public List<string> GetNames()
		{
			List<string> list = new List<string>(this.list.Count);
			foreach (Map item in this.list)
			{
				list.Add(item.name);
			}
			return list;
		}

		public void Set(string mapName)
		{
			foreach (Map item in list)
			{
				if (item.isLoaded)
				{
					item.transform.gameObject.SetActive(false);
				}
			}
			current = list.Find((Map m) => m.transform.name == mapName);
			if (current != null)
			{
				Reset(current);
			}
			else
			{
				Debug.LogError("Maps.Set: map not found (" + mapName + ")");
			}
		}

		public void Reset(Map map)
		{
			if (map == null)
			{
				return;
			}
			map.transform.gameObject.SetActive(true);
			Transform transform = map.transform.FindChild("Pickable");
			if (transform != null)
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(true);
				}
			}
		}
	}

	public class Map
	{
		public string name;

		public string path;

		public int version;

		[Ignore]
		public bool isLoaded;

		[Ignore]
		public Transform transform;

		[Ignore]
		private List<SpawnPoint> spawns;

		public Map()
		{
		}

		public Map(Transform pTransform)
		{
			name = pTransform.name;
			SetTransform(pTransform);
		}

		public void SetTransform(Transform trans)
		{
			if (trans != null && trans.name == name)
			{
				transform = trans;
				spawns = new List<SpawnPoint>(transform.GetComponentsInChildren<SpawnPoint>());
				isLoaded = true;
			}
			else
			{
				Debug.LogError("Map.SetTransform: trans is null");
			}
		}

		public Transform GetSpawnRandom(GameLogic.Team team)
		{
			List<SpawnPoint> list = spawns.FindAll((SpawnPoint s) => s.team == (SpawnPoint.Team)team);
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index].transform;
		}

		public bool isAssetBundle()
		{
			return !string.IsNullOrEmpty(path);
		}
	}

	public Server server;

	public Maps maps;

	public static bool isLocal;

	public void Load(Action<bool, string, int> cb)
	{
		maps = new Maps();
		string text = "ministrike.cfg";
		text = "/sdcard/ministrike.cfg";
		Configuration config;
		try
		{
			config = Configuration.LoadFromFile(text);
		}
		catch (Exception ex)
		{
			server.batchMode = SystemInfo.graphicsDeviceID == 0 && Application.platform != RuntimePlatform.Android;
			Debug.LogError("Config.Load: " + ex.ToString());
			Transform transform = GameObject.Find("Maps").transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				maps.Add(new Map(transform.GetChild(i)));
			}
			cb(true, string.Empty, 100);
			return;
		}
		StartCoroutine(LoadRoutine(config, cb));
	}

	public void DownloadMap(Map map, Action<bool, string, int> cb)
	{
		StartCoroutine(DownloadAndCacheMap(map, cb));
	}

	private IEnumerator LoadRoutine(Configuration config, Action<bool, string, int> cb)
	{
		foreach (Section section in config)
		{
			if (section.Name == "Server")
			{
				string version = server.version;
				int versionCode = server.versionCode;
				server = section.CreateObject<Server>();
				server.version = version;
				server.versionCode = versionCode;
				server.batchMode = SystemInfo.graphicsDeviceID == 0 && Application.platform != RuntimePlatform.Android;
				Debug.Log("Config.Load: Server configuration found");
			}
			else
			{
				if (!(section.Name == "Map"))
				{
					continue;
				}
				Map map = section.CreateObject<Map>();
				if (!map.isAssetBundle())
				{
					Transform transform = GameObject.Find("Maps").transform.FindChild(map.name);
					if (!transform)
					{
						Debug.LogError("Config.Load: built-in map \"" + map.name + "\" doesn't exist, skipping...");
						continue;
					}
					Debug.Log("Config.Load: map configuration found: " + map.name);
					map.SetTransform(transform);
					maps.Add(map);
				}
				else
				{
					Debug.Log("Config.Load: map configuration found: " + map.name + " (path=" + map.path + " - version=" + map.version + ")");
					yield return StartCoroutine(DownloadAndCacheMap(map, cb));
				}
			}
		}
		maps.current = maps.Get()[UnityEngine.Random.Range(0, maps.Get().Count)];
		cb(true, string.Empty, 100);
	}

	private IEnumerator DownloadAndCacheMap(Map map, Action<bool, string, int> cb)
	{
		while (!Caching.ready)
		{
			yield return null;
		}
		WWW www = WWW.LoadFromCacheOrDownload(map.path, map.version);
		while (!www.isDone)
		{
			cb(false, map.name, (int)(www.progress * 100f));
			yield return null;
		}
		if (www.error != null)
		{
			Debug.LogError("DownloadAndCacheMap: " + www.error);
			cb(false, www.error, 100);
			yield break;
		}
		try
		{
			AssetBundle assetBundle = www.assetBundle;
			string text = assetBundle.GetAllAssetNames()[0];
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(assetBundle.LoadAsset(text));
			if ((bool)gameObject)
			{
				gameObject.name = (map.name = gameObject.name.Replace("(Clone)", string.Empty));
				gameObject.transform.parent = GameObject.Find("Maps").transform;
				map.SetTransform(gameObject.transform);
				Transform transform = map.transform.FindChild("Bounds");
				if (!transform)
				{
					UnityEngine.Object.Destroy(gameObject);
					assetBundle.Unload(false);
					Debug.LogError("\"Bounds\" GameObject not found, skipping map (" + map.name + ")");
					yield break;
				}
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).tag = "Bounds";
				}
				gameObject.SetActive(false);
				maps.Add(map);
			}
			assetBundle.Unload(false);
		}
		catch (Exception ex)
		{
			Debug.LogError("DownloadAndCacheMap: " + ex.ToString());
			cb(false, ex.ToString(), 100);
			yield break;
		}
		cb(true, map.name, 100);
	}
}
