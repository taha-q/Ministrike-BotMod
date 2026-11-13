using UnityEngine;
using UnityEngine.SceneManagement;

public class Ui : MonoBehaviour
{
	public enum MessageType
	{
		Chat = 0,
		Info = 1,
		Damage = 2,
		Kill = 3
	}

	public enum MessageAnchor
	{
		Top = 0,
		Bottom = 1
	}

	public enum MessageLength
	{
		Short = 0,
		Long = 1
	}

	public enum UiState
	{
		None = 0,
		Hud = 1,
		MenuChooseTeam = 2,
		MenuInGame = 3,
		MenuBuy = 4,
		MenuChat = 5,
		Spectator = 6
	}

	public UiState uiState;

	public GameLogic game;

	public Hud hud;

	public Menu menu;

	public MenuServersList menuServers;

	public MenuInGame menuInGame;

	public GameObject menuChooseTeam;

	public UILabel menuChooseTeamLabel;

	public UIPlayTween menuDead;

	public UiPlayerInfo menuDeadPlayerInfo;

	public UIPlayTween menuBuy;

	public UIMessageBox box;

	public HUDText hudTextTop;

	public HUDText hudTextBottom;

	public UiPlayerInfo playerInfo;

	public UIPlayTween chatPlayTween;

	public UITextList chatTextListWindow;

	public UITextList chatTextListBox;

	public string messagesLog;

	public void Show(UiState state, string arg = "none")
	{
		Debug.Log("Ui.Show: " + state);
		Player localPlayer = game.players.GetLocalPlayer();
		switch (state)
		{
		case UiState.Hud:
			if (localPlayer == null || localPlayer.GetLife() <= 0)
			{
				Show(UiState.Spectator, "NoSwitchCam");
				break;
			}
			localPlayer.controller.cameraController.SetPlayer(localPlayer);
			hud.UpdateWeapons();
			hud.UpdateMoney();
			hud.UpdateLife();
			menuInGame.tweener.Play(false);
			menuChooseTeam.SetActive(false);
			menuDead.Play(false);
			menuBuy.Play(false);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.gameObject.SetActive(true);
			hud.playTween.Play(true);
			game.controller.Show(true);
			chatPlayTween.Play(false);
			uiState = state;
			break;
		case UiState.MenuChooseTeam:
			menuChooseTeamLabel.text = arg;
			menuInGame.tweener.Play(false);
			menuChooseTeam.SetActive(true);
			menuDead.Play(false);
			menuBuy.Play(false);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.playTween.Play(false);
			game.controller.Show(false);
			chatPlayTween.Play(false);
			uiState = state;
			break;
		case UiState.MenuInGame:
			menuInGame.gameObject.SetActive(true);
			menuInGame.tweener.Play(true);
			menuChooseTeam.SetActive(false);
			menuDead.Play(false);
			menuBuy.Play(false);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.playTween.Play(false);
			game.controller.Show(false);
			chatPlayTween.Play(false);
			uiState = state;
			break;
		case UiState.MenuBuy:
			menuInGame.tweener.Play(false);
			menuChooseTeam.SetActive(false);
			menuDead.Play(false);
			menuBuy.gameObject.SetActive(true);
			menuBuy.Play(true);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.playTween.Play(false);
			game.controller.Show(false);
			chatPlayTween.Play(false);
			uiState = state;
			break;
		case UiState.MenuChat:
			menuInGame.tweener.Play(false);
			menuChooseTeam.SetActive(false);
			menuDead.Play(false);
			menuBuy.Play(false);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.playTween.Play(false);
			game.controller.Show(false);
			chatPlayTween.Play(true);
			uiState = state;
			break;
		case UiState.Spectator:
			menuInGame.tweener.Play(false);
			menuChooseTeam.SetActive(false);
			menuDead.gameObject.SetActive(true);
			menuDead.Play(true);
			menuBuy.Play(false);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.playTween.Play(false);
			game.controller.Show(false);
			chatPlayTween.Play(false);
			uiState = state;
			break;
		default:
			menuInGame.tweener.Play(false);
			menuChooseTeam.SetActive(false);
			menuDead.Play(false);
			menuBuy.Play(false);
			hud.uiInputChatBoxPlayTween.Play(true);
			hud.playTween.Play(false);
			game.controller.Show(false);
			chatPlayTween.Play(false);
			uiState = state;
			break;
		}
	}

	public void ShowHud()
	{
		Show(UiState.Hud);
	}

	public void ShowMenuInGame()
	{
		Show(UiState.MenuInGame);
	}

	public void ShowMenuBuy()
	{
		Show(UiState.MenuBuy);
	}

	public void ShowMenuChat()
	{
		Show(UiState.MenuChat);
	}

	public void AddChatText(string msg)
	{
		chatTextListWindow.Add(msg);
		chatTextListBox.Add(msg);
	}

	public void OnLoadSceneTitle()
	{
		SceneManager.LoadScene(0);
	}

	public void OnLoadSceneMenuServersButton()
	{
		SceneManager.LoadScene(1);
	}

	public void OnStartServerButton()
	{
		menuServers.CreateServer();
	}

	public void OnSelectMapButton(string mapName)
	{
		game.config.maps.current = game.config.maps.Get(mapName);
	}

	public void SetLocalPlayerCameraTarget()
	{
		Player localPlayer = game.players.GetLocalPlayer();
		if (localPlayer != null)
		{
			localPlayer.SetCameraTarget(null);
		}
	}

	public void PlayTweenForward(UIPlayTween playTween)
	{
		playTween.Play(true);
	}

	public void PlayTweenReverse(UIPlayTween playTween)
	{
		playTween.Play(false);
	}

	public void MessageTopShort(MessageType type, object message)
	{
		Message(MessageAnchor.Top, MessageLength.Short, type, message);
	}

	public void MessageBottomShort(MessageType type, object message)
	{
		Message(MessageAnchor.Bottom, MessageLength.Short, type, message);
	}

	public void MessageTopLong(MessageType type, object message)
	{
		Message(MessageAnchor.Top, MessageLength.Long, type, message);
	}

	public void MessageBottomLong(MessageType type, object message)
	{
		Message(MessageAnchor.Bottom, MessageLength.Long, type, message);
	}

	public void Message(MessageAnchor anchor, MessageLength length, MessageType type, object message)
	{
		HUDText hUDText = ((anchor != MessageAnchor.Top) ? hudTextBottom : hudTextTop);
		float stayDuration = ((length != MessageLength.Short) ? 4f : 1f);
		switch (type)
		{
		case MessageType.Chat:
		{
			string msg3 = "[FFFF00]" + (string)message + "[-]";
			AddChatText(msg3);
			break;
		}
		case MessageType.Info:
		{
			string msg2 = "[00FF00][INFO]: " + (string)message + "[-]";
			AddChatText(msg2);
			hUDText.Add(message, Color.green, stayDuration);
			break;
		}
		case MessageType.Damage:
			hUDText.Add(message, Color.yellow, stayDuration);
			break;
		case MessageType.Kill:
		{
			string msg = "[FF0000][KILL]: " + (string)message + "[-]";
			AddChatText(msg);
			hUDText.Add(message, Color.red, stayDuration);
			break;
		}
		}
	}
}
