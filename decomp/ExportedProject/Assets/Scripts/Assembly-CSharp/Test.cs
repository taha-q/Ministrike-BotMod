using UnityEngine;

public class Test : MonoBehaviour
{
	private int unSecuredInteger = 500;

	private SecureInt securedInteger = 600;

	private float unSecuredFloat = 1.5f;

	private SecureFloat securedFloat = 2.5f;

	private string unSecuredString = "unsecuredString";

	private SecureString securedString = "secureString";

	private void OnGUI()
	{
		GUILayout.Space(32f);
		TestSecureInt();
		TestSecureFloat();
		TestSecureString();
		TestSecurePrefs();
	}

	private void TestSecureInt()
	{
		GUILayout.Label("unSecuredInteger: " + unSecuredInteger);
		if (GUILayout.Button("unSecuredInteger++"))
		{
			unSecuredInteger++;
		}
		GUILayout.Space(16f);
		GUILayout.Label("securedInteger: " + (int)securedInteger);
		if (GUILayout.Button("securedInteger++"))
		{
			++securedInteger;
		}
		GUILayout.Space(32f);
	}

	private void TestSecureFloat()
	{
		GUILayout.Label("unSecuredFloat: " + unSecuredFloat);
		if (GUILayout.Button("unSecuredFloat++"))
		{
			unSecuredFloat += 0.1f;
		}
		GUILayout.Space(16f);
		GUILayout.Label("securedFloat: " + (float)securedFloat);
		if (GUILayout.Button("securedFloat++"))
		{
			securedFloat += 0.1f;
		}
		GUILayout.Space(32f);
	}

	private void TestSecureString()
	{
		GUILayout.Label("unSecuredString: " + unSecuredString);
		if (GUILayout.Button("unSecuredString++"))
		{
			unSecuredString += "1";
		}
		GUILayout.Space(16f);
		GUILayout.Label("securedString: " + securedString);
		if (GUILayout.Button("securedString++"))
		{
			securedString += "1";
		}
		GUILayout.Space(32f);
	}

	private void TestSecurePrefs()
	{
		string text = PlayerPrefs.GetString("unSecuredPrefsString");
		GUILayout.Label("unSecuredPrefsString: " + text);
		if (GUILayout.Button("unSecuredPrefsString++"))
		{
			text += "+";
			PlayerPrefs.SetString("unSecuredPrefsString", text);
		}
		GUILayout.Space(16f);
		string text2 = SecurePrefs.GetString("securedPrefsString");
		GUILayout.Label("securedPrefsString: " + text2);
		if (GUILayout.Button("securedPrefsString++"))
		{
			text2 += "+";
			SecurePrefs.SetString("securedPrefsString", text2);
		}
		GUILayout.Space(32f);
	}
}
