using System;

[Serializable]
public struct SecureInt
{
	private string crypted;

	public SecureInt(int i)
	{
		crypted = SecureMe.Encrypt(i.ToString());
	}

	public static implicit operator SecureInt(int value)
	{
		return new SecureInt(value);
	}

	public static implicit operator int(SecureInt a)
	{
		return a.Get();
	}

	public override string ToString()
	{
		return Get().ToString();
	}

	public static SecureInt operator ++(SecureInt a)
	{
		return a.Get() + 1;
	}

	public static SecureInt operator --(SecureInt a)
	{
		return a.Get() - 1;
	}

	public static SecureInt operator +(SecureInt a, int b)
	{
		return a.Get() + b;
	}

	public static SecureInt operator -(SecureInt a, int b)
	{
		return a.Get() - b;
	}

	public static SecureInt operator *(SecureInt a, int b)
	{
		return a.Get() * b;
	}

	public static SecureInt operator /(SecureInt a, int b)
	{
		return a.Get() / b;
	}

	public static SecureInt operator %(SecureInt a, int b)
	{
		return a.Get() % b;
	}

	public static SecureInt operator +(SecureInt a, SecureInt b)
	{
		return a.Get() + b.Get();
	}

	public static SecureInt operator -(SecureInt a, SecureInt b)
	{
		return a.Get() - b.Get();
	}

	public static SecureInt operator *(SecureInt a, SecureInt b)
	{
		return a.Get() * b.Get();
	}

	public static SecureInt operator /(SecureInt a, SecureInt b)
	{
		return a.Get() / b.Get();
	}

	public static SecureInt operator %(SecureInt a, SecureInt b)
	{
		return a.Get() % b.Get();
	}

	private int Get()
	{
		if (string.IsNullOrEmpty(crypted))
		{
			return 0;
		}
		int result = 0;
		int.TryParse(SecureMe.Decrypt(crypted), out result);
		return result;
	}
}
