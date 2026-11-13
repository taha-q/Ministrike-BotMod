using System;

[Serializable]
public struct SecureString
{
	private string crypted;

	public SecureString(string s)
	{
		crypted = SecureMe.Encrypt(s);
	}

	public static implicit operator SecureString(string value)
	{
		return new SecureString(value);
	}

	public static implicit operator string(SecureString a)
	{
		return a.Get();
	}

	public static SecureString operator +(SecureString a, string b)
	{
		return a.Get() + b;
	}

	public override string ToString()
	{
		return Get().ToString();
	}

	private string Get()
	{
		if (string.IsNullOrEmpty(crypted))
		{
			return string.Empty;
		}
		return SecureMe.Decrypt(crypted);
	}
}
