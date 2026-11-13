using UnityEngine;

public class UIMessageBoxDemo : MonoBehaviour
{
	public UIMessageBox box;

	private void Start()
	{
		box.Show(UIMessageBox.UIMessageType.Info, "Title", "This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... ");
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Simple Message"))
		{
			box.Show(UIMessageBox.UIMessageType.Info, "Title", "This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... This is a simple message ... ");
		}
		if (!GUILayout.Button("Advanced Message"))
		{
			return;
		}
		box.Show(UIMessageBox.UIMessageType.Choice, "Hello World", "This is an easy message box", (UIMessageBox.UIAction action) =>
		{
			switch (action)
			{
			case UIMessageBox.UIAction.Confirm:
				Debug.Log("Confirm #1: " + action);
				box.Show(UIMessageBox.UIMessageType.Choice, "Hello World", "Well done! Click the Ok button to finish", (UIMessageBox.UIAction uIAction) =>
				{
					Debug.Log("Confirm #2: " + uIAction);
					box.Hide();
				});
				break;
			case UIMessageBox.UIAction.Cancel:
				Debug.Log("Cancel");
				box.Hide();
				break;
			case UIMessageBox.UIAction.Close:
				Debug.Log("Close");
				box.Hide();
				break;
			}
		});
	}
}
