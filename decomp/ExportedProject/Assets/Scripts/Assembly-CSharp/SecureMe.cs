using System;
using System.Security.Cryptography;
using System.Text;

public class SecureMe
{
	private static string k = "a234b6f8c012bd567l9e1n3f567g901h";

	public static string Encrypt(string toEncrypt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(k);
		byte[] bytes2 = Encoding.UTF8.GetBytes(toEncrypt);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
		return Convert.ToBase64String(array, 0, array.Length);
	}

	public static string Decrypt(string toDecrypt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(k);
		byte[] array;
		try
		{
			array = Convert.FromBase64String(toDecrypt);
		}
		catch (FormatException)
		{
			return string.Empty;
		}
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		byte[] bytes2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes2);
	}
}
