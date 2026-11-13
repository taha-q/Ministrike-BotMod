using System.Collections;
using UnityEngine;

public class UniFileBrowserExample : MonoBehaviour
{
	private string message = string.Empty;

	private float alpha = 1f;

	private char pathChar = '/';

	private void Start()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			pathChar = '\\';
		}
	}

	private void OnGUI()
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
		Color color = GUI.color;
		color.a = alpha;
		GUI.color = color;
		GUI.Label(new Rect(100f, 275f, 500f, 1000f), message);
		color.a = 1f;
		GUI.color = color;
	}

	private void OpenFile(string pathToFile)
	{
		int num = pathToFile.LastIndexOf(pathChar);
		message = "You selected file: " + pathToFile.Substring(num + 1, pathToFile.Length - num - 1);
		Fade();
	}

	private void OpenFiles(string[] pathsToFiles)
	{
		message = "You selected these files:\n";
		for (int i = 0; i < pathsToFiles.Length; i++)
		{
			int num = pathsToFiles[i].LastIndexOf(pathChar);
			message = message + pathsToFiles[i].Substring(num + 1, pathsToFiles[i].Length - num - 1) + "\n";
		}
		Fade();
	}

	private void SaveFile(string pathToFile)
	{
		int num = pathToFile.LastIndexOf(pathChar);
		message = "You're saving file: " + pathToFile.Substring(num + 1, pathToFile.Length - num - 1);
		Fade();
	}

	private void OpenFolder(string pathToFolder)
	{
		message = "You selected folder: " + pathToFolder;
		Fade();
	}

	private void Fade()
	{
		StopCoroutine("FadeAlpha");
		StartCoroutine("FadeAlpha");
	}

	private IEnumerator FadeAlpha()
	{
		alpha = 1f;
		yield return new WaitForSeconds(5f);
		for (alpha = 1f; alpha > 0f; alpha -= Time.deltaTime / 4f)
		{
			yield return null;
		}
		message = string.Empty;
	}
}
