using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class UniFileBrowserExample : MonoBehaviour
{
	[Serializable]
	internal sealed class _0024FadeAlpha_00247 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal UniFileBrowserExample _0024self__00248;

			public _0024(UniFileBrowserExample self_)
			{
				_0024self__00248 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024self__00248.alpha = 1f;
					result = (Yield(2, new WaitForSeconds(5f)) ? 1 : 0);
					break;
				case 2:
					_0024self__00248.alpha = 1f;
					goto IL_0081;
				case 3:
					_0024self__00248.alpha -= Time.deltaTime / 4f;
					goto IL_0081;
				case 1:
					{
						result = 0;
						break;
					}
					IL_0081:
					if (_0024self__00248.alpha > 0f)
					{
						result = (YieldDefault(3) ? 1 : 0);
						break;
					}
					_0024self__00248.message = string.Empty;
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal UniFileBrowserExample _0024self__00249;

		public _0024FadeAlpha_00247(UniFileBrowserExample self_)
		{
			_0024self__00249 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__00249);
		}
	}

	private string message;

	private float alpha;

	private char pathChar;

	public UniFileBrowserExample()
	{
		message = string.Empty;
		alpha = 1f;
		pathChar = "/"[0];
	}

	public virtual void Start()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			pathChar = "\\"[0];
		}
	}

	public virtual void OnGUI()
	{
		if (GUI.Button(new Rect(100f, 50f, 95f, 35f), "Open"))
		{
			if (UniFileBrowser.use.allowMultiSelect)
			{
				UniFileBrowser.use.OpenFileWindow(OpenFiles);
			}
			else
			{
				UniFileBrowser.use.OpenFileWindow(OpenFile);
			}
		}
		if (GUI.Button(new Rect(100f, 125f, 95f, 35f), "Save"))
		{
			UniFileBrowser.use.SaveFileWindow(SaveFile);
		}
		if (GUI.Button(new Rect(100f, 200f, 95f, 35f), "Open Folder"))
		{
			UniFileBrowser.use.OpenFolderWindow(true, OpenFolder);
		}
		float a = alpha;
		Color color = GUI.color;
		float num = (color.a = a);
		Color color2 = (GUI.color = color);
		GUI.Label(new Rect(100f, 275f, 500f, 1000f), message);
		float a2 = 1f;
		Color color4 = GUI.color;
		float num2 = (color4.a = a2);
		Color color5 = (GUI.color = color4);
	}

	public virtual void OpenFile(string pathToFile)
	{
		message = "You selected file: " + pathToFile.Substring(pathToFile.LastIndexOf(pathChar) + 1);
		Fade();
	}

	public virtual void OpenFiles(string[] pathsToFiles)
	{
		message = "You selected these files:\n";
		for (int i = 0; i < pathsToFiles.Length; i++)
		{
			message += pathsToFiles[i].Substring(pathsToFiles[i].LastIndexOf(pathChar) + 1) + "\n";
		}
		Fade();
	}

	public virtual void SaveFile(string pathToFile)
	{
		message = "You're saving file: " + pathToFile.Substring(pathToFile.LastIndexOf(pathChar) + 1);
		Fade();
	}

	public virtual void OpenFolder(string pathToFolder)
	{
		message = "You selected folder: " + pathToFolder;
		Fade();
	}

	public virtual void Fade()
	{
		StopCoroutine("FadeAlpha");
		StartCoroutine("FadeAlpha");
	}

	public virtual IEnumerator FadeAlpha()
	{
		return new _0024FadeAlpha_00247(this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
