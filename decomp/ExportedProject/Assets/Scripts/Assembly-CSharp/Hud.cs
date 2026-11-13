using System;
using System.Collections;
using UnityEngine;

public class Hud : MonoBehaviour
{
	public GameLogic game;

	public GameObject buyButton;

	public GameObject snipeButton;

	public GameObject scopeSprite;

	public UILabel uiLabelTime;

	public UILabel uiLabelLife;

	public UILabel uiLabelMoney;

	public UILabel uiLabelShield;

	public UIInput uiInputChat;

	public UIPlayTween uiInputChatBoxPlayTween;

	public UiWeapon uiWeaponPrimary;

	public UiWeapon uiWeaponSecondary;

	public UISprite uiSpriteGrenade;

	public UIPlayTween playTween;

	private void OnEnable()
	{
		StartCoroutine(UpdateHud());
	}

	public void UpdateMoney()
	{
		uiLabelMoney.text = "$ " + game.players.GetLocalPlayer().GetInventory().GetMoney();
	}

	public void UpdateLife()
	{
		uiLabelLife.text = game.players.GetLocalPlayer().GetLife().ToString();
		uiLabelShield.text = game.players.GetLocalPlayer().GetShield().ToString();
	}

	public void UpdateWeapons()
	{
		Inventory.Slot slot = game.players.GetLocalPlayer().GetInventory().GetSlot();
		uiWeaponPrimary.Enable(slot == Inventory.Slot.Primary);
		uiWeaponSecondary.Enable(slot == Inventory.Slot.Secondary);
		Weapon weapon = game.players.GetLocalPlayer().GetInventory().GetWeapon(Inventory.Slot.Primary);
		if (weapon != null)
		{
			uiWeaponPrimary.uiSpriteWeapon.spriteName = weapon.model.ToString();
			if (slot == Inventory.Slot.Primary && weapon.model == Weapon.Model.AWP)
			{
				snipeButton.active = true;
			}
		}
		else
		{
			uiWeaponPrimary.uiLabelBullets.text = "00/00";
			snipeButton.active = false;
			scopeSprite.active = false;
			Camera.main.fieldOfView = 60f;
		}
		UpdateBullets(weapon);
		Weapon weapon2 = game.players.GetLocalPlayer().GetInventory().GetWeapon(Inventory.Slot.Secondary);
		if (weapon2 != null)
		{
			uiWeaponSecondary.uiSpriteWeapon.spriteName = weapon2.model.ToString();
		}
		else
		{
			uiWeaponSecondary.uiLabelBullets.text = "00/00";
		}
		UpdateBullets(weapon2);
	}

	public void UpdateBullets(Weapon weapon)
	{
		if (weapon != null)
		{
			UILabel uILabel = ((weapon.slot != Inventory.Slot.Primary) ? uiWeaponSecondary.uiLabelBullets : uiWeaponPrimary.uiLabelBullets);
			uILabel.text = weapon.GetBullets() + "/" + weapon.GetChargers() * weapon.bulletsStock;
		}
	}

	private IEnumerator UpdateHud()
	{
		while (base.gameObject.activeSelf)
		{
			yield return new WaitForSeconds(1f);
			uiLabelTime.text = game.GetRemainingTime();
			if (game.config.server.timePerRound - game.timeRemainingRound <= 30 && !buyButton.activeSelf && game.players.GetLocalPlayer() != null && game.players.GetLocalPlayer().GetTeam() != GameLogic.Team.Spectator)
			{
				buyButton.SetActive(true);
			}
			else if (game.config.server.timePerRound - game.timeRemainingRound > 30 && buyButton.activeSelf)
			{
				buyButton.SetActive(false);
			}
			if (snipeButton.activeSelf)
			{
				Weapon weapon = game.players.GetLocalPlayer().GetInventory().GetWeapon(Inventory.Slot.Primary);
				if (weapon == null || weapon.model != Weapon.Model.AWP)
				{
					snipeButton.active = false;
					Camera.main.fieldOfView = 60f;
				}
			}
			else
			{
				scopeSprite.active = false;
			}
		}
	}

	public void OnButtonWeaponPrimary()
	{
		Player localPlayer = game.players.GetLocalPlayer();
		Inventory.Slot slot = localPlayer.GetInventory().GetSlot();
		Weapon weapon = localPlayer.GetInventory().GetWeapon(Inventory.Slot.Primary);
		if (!(weapon != null))
		{
			return;
		}
		if (slot == Inventory.Slot.Primary)
		{
			if (weapon.GetBullets() < weapon.bulletsStock)
			{
				weapon.Reload();
				game.ui.hud.UpdateWeapons();
			}
		}
		else
		{
			uiWeaponPrimary.Enable(true);
			uiWeaponSecondary.Enable(false);
			localPlayer.GetInventory().SetSlot(Inventory.Slot.Primary);
			snipeButton.active = weapon.model == Weapon.Model.AWP;
		}
	}

	public void OnButtonWeaponSecondary()
	{
		Player localPlayer = game.players.GetLocalPlayer();
		Inventory.Slot slot = localPlayer.GetInventory().GetSlot();
		Weapon weapon = localPlayer.GetInventory().GetWeapon(Inventory.Slot.Secondary);
		if (!(weapon != null))
		{
			return;
		}
		if (slot == Inventory.Slot.Secondary)
		{
			if (weapon.GetBullets() < weapon.bulletsStock)
			{
				weapon.Reload();
				game.ui.hud.UpdateWeapons();
			}
		}
		else
		{
			uiWeaponPrimary.Enable(false);
			uiWeaponSecondary.Enable(true);
			localPlayer.GetInventory().SetSlot(Inventory.Slot.Secondary);
			snipeButton.active = false;
			scopeSprite.active = false;
			Camera.main.fieldOfView = 60f;
		}
	}

	public void OnButtonKnife()
	{
		game.players.GetLocalPlayer().GetInventory().SetSlot(Inventory.Slot.Knife);
		uiWeaponPrimary.Enable(false);
		uiWeaponSecondary.Enable(false);
		snipeButton.active = false;
		scopeSprite.active = false;
		Camera.main.fieldOfView = 60f;
	}

	public void OnButtonDrop()
	{
		game.players.GetLocalPlayer().GetInventory().DropWeapon();
		snipeButton.active = false;
		scopeSprite.active = false;
		Camera.main.fieldOfView = 60f;
	}

	public void OnButtonGrenade()
	{
		Player localPlayer = game.players.GetLocalPlayer();
		if (localPlayer.GetInventory().grenades > 0 && !localPlayer.animationController.IsPlayingHe())
		{
			NetworkViewID viewID = Network.AllocateViewID();
			localPlayer.ShootGrenade(viewID);
		}
	}

	public void OnButtonSnipe()
	{
		switch ((int)Camera.main.fieldOfView)
		{
		case 60:
			Camera.main.fieldOfView = 25f;
			scopeSprite.active = true;
			break;
		case 25:
			Camera.main.fieldOfView = 5f;
			scopeSprite.active = true;
			break;
		default:
			Camera.main.fieldOfView = 60f;
			scopeSprite.active = false;
			break;
		}
	}

	public void OnButtonCelebrate1()
	{
		Player localPlayer = game.players.GetLocalPlayer();
		if (localPlayer != null && localPlayer.IsAlive())
		{
			localPlayer.animationController.Play(AnimationController.AnimState.Celebrate1);
			game.netView.RPC("RPCAnim", RPCMode.Others, localPlayer.GetNetworkPlayerID(), 13);
			game.ui.MessageTopShort(Ui.MessageType.Chat, localPlayer.name + ": you f*ck my wife ?!");
		}
	}

	public void OnButtonCelebrate2()
	{
		Player localPlayer = game.players.GetLocalPlayer();
		if (localPlayer != null && localPlayer.IsAlive())
		{
			localPlayer.animationController.Play(AnimationController.AnimState.Celebrate2);
			game.netView.RPC("RPCAnim", RPCMode.Others, localPlayer.GetNetworkPlayerID(), 14);
			game.ui.MessageTopShort(Ui.MessageType.Chat, localPlayer.name + ": Yippee ki-yay !");
		}
	}

	public void OnButtonMessage(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}
		if (message.StartsWith("/"))
		{
			string[] array = message.Split(' ');
			int num = array.Length;
			if (num <= 1)
			{
				return;
			}
			try
			{
				if (Network.isServer)
				{
					game.rpc.RPCRcon(array[0].Remove(0, 1), array[1], (num <= 2) ? string.Empty : array[2]);
					return;
				}
				game.netView.RPC("RPCRcon", RPCMode.Server, array[0].Remove(0, 1), array[1], (num <= 2) ? string.Empty : array[2]);
				return;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return;
			}
		}
		game.netView.RPC("RPCMessage", RPCMode.All, game.players.GetLocalPlayer().GetNetworkPlayerID(), message);
	}
}
