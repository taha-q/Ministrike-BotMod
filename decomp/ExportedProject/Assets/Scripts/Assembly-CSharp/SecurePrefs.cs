using UnityEngine;

public class SecurePrefs
{
	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	public static SecureFloat GetFloat(string key)
	{
		return GetFloat(key, 0f);
	}

	public static SecureFloat GetFloat(string key, float defaultValue)
	{
		string text = PlayerPrefs.GetString(key);
		if (string.IsNullOrEmpty(text))
		{
			return defaultValue;
		}
		string text2 = SecureMe.Decrypt(text);
		if (string.IsNullOrEmpty(text2))
		{
			return defaultValue;
		}
		float result = defaultValue;
		float.TryParse(text2, out result);
		return new SecureFloat(result);
	}

	public static SecureInt GetInt(string key)
	{
		return GetInt(key, 0);
	}

	public static SecureInt GetInt(string key, int defaultValue)
	{
		string text = PlayerPrefs.GetString(key);
		if (string.IsNullOrEmpty(text))
		{
			return defaultValue;
		}
		string text2 = SecureMe.Decrypt(text);
		if (string.IsNullOrEmpty(text2))
		{
			return defaultValue;
		}
		int result = defaultValue;
		int.TryParse(text2, out result);
		return new SecureInt(result);
	}

	public static SecureString GetString(string key)
	{
		return GetString(key, string.Empty);
	}

	public static SecureString GetString(string key, string defaultValue)
	{
		string text = PlayerPrefs.GetString(key);
		if (string.IsNullOrEmpty(text))
		{
			return defaultValue;
		}
		string text2 = SecureMe.Decrypt(text);
		if (string.IsNullOrEmpty(text2))
		{
			return defaultValue;
		}
		return new SecureString(text2);
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public static void Save()
	{
		PlayerPrefs.Save();
	}

	public static void SetFloat(string key, float value)
	{
		string value2 = SecureMe.Encrypt(value.ToString());
		PlayerPrefs.SetString(key, value2);
	}

	public static void SetInt(string key, int value)
	{
		string value2 = SecureMe.Encrypt(value.ToString());
		PlayerPrefs.SetString(key, value2);
	}

	public static void SetString(string key, string value)
	{
		string value2 = SecureMe.Encrypt(value);
		PlayerPrefs.SetString(key, value2);
	}
}
