using System;
using System.Collections;
using UnityEngine;

public class IpLocation : MonoBehaviour
{
	public string ip;

	public string hostname;

	public string city;

	public string region;

	public string country;

	public string loc;

	public string org;

	public void UpdateLocation(Action<bool> action)
	{
		StartCoroutine(UpdateLocationRoutine(action));
	}

	private IEnumerator UpdateLocationRoutine(Action<bool> action)
	{
		string url = "http://ipinfo.io/json";
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.text) || !www.text.Contains("country"))
		{
			Debug.LogError("UpdateLocation www.error: " + www.error);
			Debug.LogError("UpdateLocation www.text: " + www.text);
			if (action != null)
			{
				action(false);
			}
			yield break;
		}
		ip = getString(www.text, "ip");
		Debug.Log("ip: " + ip);
		hostname = getString(www.text, "hostname");
		Debug.Log("hostname: " + hostname);
		city = getString(www.text, "city");
		Debug.Log("city: " + city);
		region = getString(www.text, "region");
		Debug.Log("region: " + region);
		country = getString(www.text, "country");
		Debug.Log("country: " + country);
		loc = getString(www.text, "loc");
		Debug.Log("loc: " + loc);
		org = getString(www.text, "org");
		Debug.Log("org: " + org);
		if (action != null)
		{
			action(true);
		}
	}

	private string getString(string jsonText, string value)
	{
		if (!jsonText.Contains(value))
		{
			return string.Empty;
		}
		int num = jsonText.IndexOf(value) + value.Length + 2;
		int length = jsonText.IndexOf("\n", num) - num;
		return jsonText.Substring(num, length).Replace("\"", string.Empty).Replace(",", string.Empty)
			.Trim();
	}
}
