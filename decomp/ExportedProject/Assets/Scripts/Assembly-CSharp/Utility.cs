using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Utility
{
	private static string k = "84B534DA932C4781CDFBDAEB";

	public static string Encrypt(byte[] toEncrypt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(k);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] inArray = cryptoTransform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
		return Convert.ToBase64String(inArray);
	}

	public static byte[] Decrypt(string toDecrypt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(k);
		byte[] array = Convert.FromBase64String(toDecrypt);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		return cryptoTransform.TransformFinalBlock(array, 0, array.Length);
	}

	public static string serialize(object serializableObject)
	{
		if (serializableObject == null)
		{
			Debug.Log("serialize: serializableObject == null");
			return null;
		}
		if (!serializableObject.GetType().IsSerializable)
		{
			Debug.Log("serialize: not serializable");
			return null;
		}
		MemoryStream memoryStream = new MemoryStream();
		new BinaryFormatter().Serialize(memoryStream, serializableObject);
		return Encrypt(memoryStream.ToArray());
	}

	public static object unserialize(string serializedString)
	{
		if (serializedString == null || serializedString == string.Empty || serializedString.Equals("null"))
		{
			return null;
		}
		MemoryStream memoryStream = null;
		byte[] buffer = null;
		try
		{
			buffer = Decrypt(serializedString);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		memoryStream = new MemoryStream(buffer);
		return new BinaryFormatter().Deserialize(memoryStream);
	}

	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			transform.gameObject.layer = layerNumber;
		}
	}

	public static string ObjectToString(object obj)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			new BinaryFormatter().Serialize(memoryStream, obj);
			return Convert.ToBase64String(memoryStream.ToArray());
		}
	}

	public static object StringToObject(string base64String)
	{
		byte[] array = Convert.FromBase64String(base64String);
		using (MemoryStream memoryStream = new MemoryStream(array, 0, array.Length))
		{
			memoryStream.Write(array, 0, array.Length);
			memoryStream.Position = 0L;
			return new BinaryFormatter().Deserialize(memoryStream);
		}
	}
}
