using System.Collections.Generic;
using UnityEngine;

public class MenuGrid : MonoBehaviour
{
	public GameLogic game;

	public UIGrid grid;

	public GameObject gridItem;

	public GameObject home;

	private Menu.Content content = Menu.Content.Primary;

	public void SetContent(Menu.Content c)
	{
		content = c;
		for (int num = grid.transform.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(grid.transform.GetChild(num).gameObject);
		}
		switch (content)
		{
		case Menu.Content.Home:
			home.SetActive(true);
			break;
		case Menu.Content.Primary:
		case Menu.Content.Secondary:
		{
			home.SetActive(false);
			List<Transform> weaponsPrefabs = game.players.GetLocalPlayer().GetInventory().weaponsPrefabs;
			foreach (Transform item in weaponsPrefabs)
			{
				Weapon component = item.GetComponent<Weapon>();
				Inventory.Slot slot = ((content == Menu.Content.Primary) ? Inventory.Slot.Primary : Inventory.Slot.Secondary);
				if (component.slot == slot)
				{
					GameObject gameObject2 = grid.gameObject.AddChild(gridItem);
					gameObject2.name = component.model.ToString();
					gameObject2.transform.FindChild("NameLabel").GetComponent<UILabel>().text = component.model.ToString();
					gameObject2.transform.FindChild("PriceLabel").GetComponent<UILabel>().text = component.price + "$";
					gameObject2.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = component.model.ToString();
					gameObject2.GetComponent<UIButtonMessage>().target = base.gameObject;
				}
			}
			break;
		}
		case Menu.Content.Equipment:
		{
			home.SetActive(false);
			GameObject gameObject = grid.gameObject.AddChild(gridItem);
			gameObject.name = "PrimaryAmmo";
			gameObject.transform.FindChild("NameLabel").GetComponent<UILabel>().text = "Primary Ammo";
			gameObject.transform.FindChild("PriceLabel").GetComponent<UILabel>().text = "500$";
			gameObject.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = "bullets";
			gameObject.GetComponent<UIButtonMessage>().target = base.gameObject;
			gameObject = grid.gameObject.AddChild(gridItem);
			gameObject.name = "SecondaryAmmo";
			gameObject.transform.FindChild("NameLabel").GetComponent<UILabel>().text = "Secondary Ammo";
			gameObject.transform.FindChild("PriceLabel").GetComponent<UILabel>().text = "300$";
			gameObject.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = "bullets";
			gameObject.GetComponent<UIButtonMessage>().target = base.gameObject;
			gameObject = grid.gameObject.AddChild(gridItem);
			gameObject.name = "Grenade";
			gameObject.transform.FindChild("NameLabel").GetComponent<UILabel>().text = "Grenade";
			gameObject.transform.FindChild("PriceLabel").GetComponent<UILabel>().text = "300$";
			gameObject.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = "Grenade";
			gameObject.GetComponent<UIButtonMessage>().target = base.gameObject;
			gameObject = grid.gameObject.AddChild(gridItem);
			gameObject.name = "Kevlar";
			gameObject.transform.FindChild("NameLabel").GetComponent<UILabel>().text = "Kevlar";
			gameObject.transform.FindChild("PriceLabel").GetComponent<UILabel>().text = "800$";
			gameObject.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = "shield";
			gameObject.GetComponent<UIButtonMessage>().target = base.gameObject;
			break;
		}
		}
		grid.Reposition();
	}

	public Menu.Content GetContent()
	{
		return content;
	}

	public void OnButtonHome()
	{
		SetContent(Menu.Content.Home);
	}

	public void OnButtonPrimary()
	{
		SetContent(Menu.Content.Primary);
	}

	public void OnButtonSecondary()
	{
		SetContent(Menu.Content.Secondary);
	}

	public void OnButtonEquipment()
	{
		SetContent(Menu.Content.Equipment);
	}

	public void OnItemButton(GameObject item)
	{
		if (game.config.server.timePerRound - game.timeRemainingRound > 30)
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "Too late !");
			return;
		}
		Player localPlayer = game.players.GetLocalPlayer();
		if (localPlayer == null)
		{
			Debug.LogWarning("Local player not found");
			return;
		}
		Inventory inventory = localPlayer.GetInventory();
		string s = item.transform.FindChild("PriceLabel").GetComponent<UILabel>().text.Replace("$", string.Empty);
		int num = int.Parse(s);
		if (inventory.GetMoney() < num)
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "Not enough money !");
		}
		else if (item.name.Equals("PrimaryAmmo") || item.name.Equals("SecondaryAmmo"))
		{
			Weapon weapon = inventory.GetWeapon(item.name.Equals("PrimaryAmmo") ? Inventory.Slot.Primary : Inventory.Slot.Secondary);
			if (weapon == null)
			{
				game.ui.MessageTopLong(Ui.MessageType.Info, "No weapon for that!");
				return;
			}
			if (weapon.GetBullets() >= weapon.bulletsStock && weapon.GetChargers() >= weapon.chargersStock)
			{
				game.ui.MessageTopLong(Ui.MessageType.Info, "You are full of ammo !");
				return;
			}
			weapon.Recharge();
			inventory.SetMoney(inventory.GetMoney() - num);
			game.ui.hud.UpdateWeapons();
			game.ui.hud.UpdateMoney();
			SetContent(Menu.Content.Home);
		}
		else if (item.name.Equals("Grenade"))
		{
			if (localPlayer.GetInventory().grenades >= 1)
			{
				game.ui.MessageTopLong(Ui.MessageType.Info, "You already own this item !");
				return;
			}
			localPlayer.GetInventory().grenades = 1;
			inventory.SetMoney(inventory.GetMoney() - num);
			game.ui.hud.UpdateMoney();
			SetContent(Menu.Content.Home);
		}
		else if (item.name.Equals("Kevlar"))
		{
			if (localPlayer.GetShield() >= 100)
			{
				game.ui.MessageTopLong(Ui.MessageType.Info, "Your Kevlar is not damaged !");
				return;
			}
			localPlayer.SetShield(100);
			inventory.SetMoney(inventory.GetMoney() - num);
			game.ui.hud.UpdateLife();
			game.ui.hud.UpdateMoney();
			SetContent(Menu.Content.Home);
		}
		else if ((bool)inventory.Find(item.name))
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "You already own this item !");
		}
		else if (localPlayer.GetTeam() == GameLogic.Team.Terro && item.name.Equals("M4A1"))
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "Only a counter can buy this gun !");
		}
		else if (localPlayer.GetTeam() == GameLogic.Team.Counter && item.name.Equals("AK47"))
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "Only a terro can buy this gun !");
		}
		else
		{
			inventory.AddWeapon(item.name);
			inventory.SetMoney(inventory.GetMoney() - num);
			game.ui.hud.UpdateWeapons();
			game.ui.hud.UpdateMoney();
			SetContent(Menu.Content.Home);
		}
	}
}
