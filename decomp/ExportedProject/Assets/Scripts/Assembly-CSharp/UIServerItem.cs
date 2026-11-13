using System;
using UnityEngine;

public class UIServerItem : MonoBehaviour
{
	public UILabel playersLabel;

	public UILabel mapLabel;

	public UILabel nameLabel;

	public UILabel countryLabel;

	public UISprite countrySprite;

	public int versionCode;

	public string mapPath;

	public int mapVersion;

	private HostData hostData;

	private string country = "aa";

	private string comment;

	private void Start()
	{
		UIWidget component = GetComponent<UIWidget>();
		component.leftAnchor.Set(base.transform.parent.parent, 0f, 0f);
		component.rightAnchor.Set(base.transform.parent.parent, 1f, 0f);
		component.ResetAndUpdateAnchors();
	}

	public HostData GetHostData()
	{
		return hostData;
	}

	public void SetHostData(HostData data)
	{
		if (data == null)
		{
			playersLabel.text = "00/00";
			mapLabel.text = "ERROR";
			nameLabel.text = "ERROR";
			return;
		}
		hostData = data;
		playersLabel.text = GetPlayers() + "/" + GetPlayersMax();
		mapLabel.text = GetMap();
		string[] array = hostData.comment.Split('|');
		if (array.Length == 1)
		{
			nameLabel.text = (comment = hostData.comment);
			return;
		}
		if (array.Length == 2)
		{
			nameLabel.text = (comment = array[1]);
			country = array[0];
			countrySprite.spriteName = country;
			return;
		}
		if (array.Length == 3)
		{
			nameLabel.text = (comment = array[1]);
			country = array[0];
			countrySprite.spriteName = country;
			try
			{
				versionCode = int.Parse(array[2]);
				return;
			}
			catch (Exception)
			{
				return;
			}
		}
		if (array.Length == 4)
		{
			nameLabel.text = (comment = array[1]);
			country = array[0];
			countrySprite.spriteName = country;
			try
			{
				versionCode = int.Parse(array[2]);
			}
			catch (Exception)
			{
			}
			mapPath = array[3];
		}
		else if (array.Length == 5)
		{
			nameLabel.text = (comment = array[1]);
			country = array[0];
			countrySprite.spriteName = country;
			try
			{
				versionCode = int.Parse(array[2]);
			}
			catch (Exception)
			{
			}
			mapPath = array[3];
			try
			{
				mapVersion = int.Parse(array[4]);
			}
			catch (Exception)
			{
			}
		}
	}

	public string GetCountry()
	{
		return country;
	}

	public string GetMap()
	{
		if (hostData == null)
		{
			return string.Empty;
		}
		return hostData.gameName;
	}

	public string GetComment()
	{
		return comment;
	}

	public int GetPlayers()
	{
		if (hostData == null)
		{
			return 0;
		}
		return hostData.connectedPlayers;
	}

	public int GetPlayersMax()
	{
		if (hostData == null)
		{
			return 0;
		}
		return hostData.playerLimit;
	}
}
