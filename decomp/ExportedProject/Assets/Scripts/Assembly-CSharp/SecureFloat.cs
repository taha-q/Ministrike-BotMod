using System;

[Serializable]
public struct SecureFloat
{
	private string crypted;

	public SecureFloat(float f)
	{
		crypted = SecureMe.Encrypt(f.ToString());
	}

	public static implicit operator SecureFloat(float value)
	{
		return new SecureFloat(value);
	}

	public static implicit operator float(SecureFloat a)
	{
		return a.Get();
	}

	public override string ToString()
	{
		return Get().ToString();
	}

	public static SecureFloat operator ++(SecureFloat a)
	{
		return a.Get() + 1f;
	}

	public static SecureFloat operator --(SecureFloat a)
	{
		return a.Get() - 1f;
	}

	public static SecureFloat operator +(SecureFloat a, float b)
	{
		return a.Get() + b;
	}

	public static SecureFloat operator -(SecureFloat a, float b)
	{
		return a.Get() - b;
	}

	public static SecureFloat operator *(SecureFloat a, float b)
	{
		return a.Get() * b;
	}

	public static SecureFloat operator /(SecureFloat a, float b)
	{
		return a.Get() / b;
	}

	public static SecureFloat operator %(SecureFloat a, float b)
	{
		return a.Get() % b;
	}

	public static SecureFloat operator +(SecureFloat a, SecureFloat b)
	{
		return a.Get() + b.Get();
	}

	public static SecureFloat operator -(SecureFloat a, SecureFloat b)
	{
		return a.Get() - b.Get();
	}

	public static SecureFloat operator *(SecureFloat a, SecureFloat b)
	{
		return a.Get() * b.Get();
	}

	public static SecureFloat operator /(SecureFloat a, SecureFloat b)
	{
		return a.Get() / b.Get();
	}

	public static SecureFloat operator %(SecureFloat a, SecureFloat b)
	{
		return a.Get() % b.Get();
	}

	private float Get()
	{
		if (string.IsNullOrEmpty(crypted))
		{
			return 0f;
		}
		float result = 0f;
		float.TryParse(SecureMe.Decrypt(crypted), out result);
		return result;
	}
}
