using UnityEngine;

public class Menu : MonoBehaviour
{
	public enum Content
	{
		Home = 0,
		Primary = 1,
		Secondary = 2,
		Equipment = 3
	}

	public GameLogic game;

	public MenuGrid grid;
}
