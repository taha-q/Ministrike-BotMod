using UnityEngine;

public class Chat : MonoBehaviour
{
	public UITextList textListChatWindow;

	public UITextList textListChatBox;

	public void Add(string msg)
	{
		textListChatWindow.Add(msg);
		textListChatBox.Add(msg);
	}
}
