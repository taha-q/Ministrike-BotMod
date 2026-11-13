using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class SaveData
{
	public SecureInt kills;

	public SecureInt deaths;

	public List<SecureInt> intList;

	public List<SecureString> stringList;

	public static byte[] ToByteArray(SaveData data)
	{
		try
		{
			string s = Utility.serialize(data);
			return Encoding.Unicode.GetBytes(s);
		}
		catch (Exception)
		{
			Debug.LogError("ToByteArray");
			return null;
		}
	}

	public static SaveData FromByteArray(byte[] array)
	{
		try
		{
			string serializedString = Encoding.Unicode.GetString(array);
			return (SaveData)Utility.unserialize(serializedString);
		}
		catch (Exception)
		{
			Debug.LogError("FromByteArray");
			return null;
		}
	}

	public override string ToString()
	{
		string text = "==== SaveData ====\n";
		string text2 = text;
		text = string.Concat(text2, "kills: ", kills, "\n");
		text2 = text;
		text = string.Concat(text2, "deaths: ", deaths, "\n");
		return text + "==== SaveData ====\n";
	}
}
