using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public enum Slot
	{
		Knife = 0,
		Primary = 1,
		Secondary = 2,
		Grenade = 3
	}

	[Serializable]
	public class InventoryData
	{
		public int slot;

		public List<Weapon.WeaponData> weapons;

		public InventoryData()
		{
			slot = 2;
			weapons = new List<Weapon.WeaponData>();
		}

		public override string ToString()
		{
			string text = "Inventory: \n";
			foreach (Weapon.WeaponData weapon in weapons)
			{
				text += "---------------\n";
				string text2 = text;
				text = string.Concat(text2, "type: ", (Weapon.Model)weapon.type, "\n");
				text2 = text;
				text = text2 + "chargers: " + weapon.chargers + "\n";
				text2 = text;
				text = text2 + "bullets: " + weapon.bullets + "\n";
				text += "---------------";
			}
			return text;
		}
	}

	public List<Transform> weaponsPrefabs;

	public GameObject grenade;

	public int moneyDefault = 800;

	private Player player;

	private int money;

	private InventoryData data = new InventoryData();

	public List<Weapon> weapons = new List<Weapon>();

	private Weapon.Model weaponDefault;

	private int grenadesCount = 1;

	private int smokesCount = 1;

	public int grenades
	{
		get
		{
			return grenadesCount;
		}
		set
		{
			grenadesCount = value;
			if (player.isMine)
			{
				if (grenadesCount <= 0)
				{
					player.game.ui.hud.uiSpriteGrenade.spriteName = "GrenadeEmpty";
				}
				else
				{
					player.game.ui.hud.uiSpriteGrenade.spriteName = "Grenade";
				}
			}
		}
	}

	public int smokes
	{
		get
		{
			return smokesCount;
		}
		set
		{
			smokesCount = value;
			if (player.isMine)
			{
				if (smokesCount <= 0)
				{
					player.game.ui.hud.uiSpriteGrenade.spriteName = "GrenadeEmpty";
				}
				else
				{
					player.game.ui.hud.uiSpriteGrenade.spriteName = "Grenade";
				}
			}
		}
	}

	public void SetPlayer(Player p)
	{
		player = p;
		money = moneyDefault;
	}

	public void Reset(bool resetMoney)
	{
		if (resetMoney)
		{
			SetMoney(moneyDefault);
		}
		data = new InventoryData();
		foreach (Weapon weapon in weapons)
		{
			UnityEngine.Object.DestroyImmediate(weapon.gameObject);
		}
		weapons.Clear();
		AddWeaponSecondary();
		SetSlot(Slot.Secondary);
	}

	public int GetMoney()
	{
		return money;
	}

	public void SetMoney(int m)
	{
		money = Mathf.Min(m, 16000);
	}

	public void SetSlot(Slot slot)
	{
		Debug.Log(player.name + ".SetSlot: " + slot);
		switch (slot)
		{
		case Slot.Grenade:
			return;
		case Slot.Knife:
		{
			player.modelController.model.knife.gameObject.SetActive(true);
			player.animationController.SetAnimType(AnimationController.AnimType.Knife);
			Weapon weapon = GetWeapon();
			if (weapon != null)
			{
				weapon.gameObject.SetActive(false);
			}
			data.slot = (int)slot;
			if (player.isMine)
			{
				player.game.netView.RPC("RPCSetSlot", RPCMode.Others, player.GetNetworkPlayerID(), data.slot);
				player.game.ui.hud.UpdateWeapons();
			}
			return;
		}
		}
		Weapon weapon2 = GetWeapon(slot);
		if (weapon2 != null)
		{
			player.modelController.model.knife.gameObject.SetActive(false);
			player.animationController.SetAnimType(weapon2);
			Weapon weapon3 = GetWeapon();
			if (weapon3 != null)
			{
				weapon3.gameObject.SetActive(false);
			}
			weapon2.gameObject.SetActive(true);
			data.slot = (int)slot;
			if (player.isMine)
			{
				player.game.netView.RPC("RPCSetSlot", RPCMode.Others, player.GetNetworkPlayerID(), data.slot);
				player.game.ui.hud.UpdateWeapons();
			}
		}
	}

	public Slot GetSlot()
	{
		return (Slot)data.slot;
	}

	public Weapon AddWeaponSecondary()
	{
		return AddWeapon(weaponDefault.ToString());
	}

	public Weapon AddWeapon(string weaponName)
	{
		Transform transform = null;
		Weapon weapon = null;
		bool flag = true;
		Debug.Log("AddWeapon: " + player.name + " weapon: " + weaponName);
		transform = weaponsPrefabs.Find((Transform w) => w.name == weaponName);
		Transform transform2 = weaponsPrefabs.Find((Transform w) => w.name == weaponName);
		if (transform2 != null)
		{
			transform = UnityEngine.Object.Instantiate(transform2);
			transform.name = transform2.name + "-" + player.GetNetworkPlayerID();
		}
		else
		{
			transform = player.game.droppedWeapons.FindChild(weaponName);
			flag = false;
			if (transform == null)
			{
				string text = weaponName.Split('-')[0];
				if (text != null)
				{
					return AddWeapon(text);
				}
			}
		}
		if (transform != null)
		{
			Transform child = transform.GetChild(0).GetChild(0);
			child.GetComponent<Collider>().enabled = false;
			child.GetComponent<Rigidbody>().velocity = Vector3.zero;
			child.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			child.GetComponent<Rigidbody>().useGravity = false;
			weapon = transform.GetComponent<Weapon>();
			weapon.player = player;
			weapon.SetHand(player.modelController.model.handR);
			Weapon weapon2 = weapons.Find((Weapon w) => w.slot == weapon.slot);
			if ((bool)weapon2)
			{
				Debug.Log(string.Concat("Removing weapon in ", weapon.slot, " slot"));
				RemoveWeapon(weapon2.name);
			}
			weapons.Add(weapon);
			if (flag && data.weapons.Find((Weapon.WeaponData w) => w.type == (int)weapon.model) == null)
			{
				Weapon.WeaponData weaponData = new Weapon.WeaponData();
				weaponData.type = (int)weapon.model;
				weaponData.bullets = weapon.bulletsStock;
				weaponData.chargers = weapon.chargersStock - 1;
				weapon.SetData(weaponData);
				data.weapons.Add(weapon.GetData());
			}
			if (data.slot != (int)weapon.slot)
			{
				weapon.gameObject.SetActive(false);
			}
		}
		if (weapon != null)
		{
			if (weapon.GetBullets() < 0)
			{
				weapon.SetBullets(0);
			}
			if (player.isMine)
			{
				player.game.ui.hud.UpdateWeapons();
				player.game.netView.RPC("RPCAddWeapon", RPCMode.Others, player.GetNetworkPlayerID(), weaponName);
			}
			return weapon;
		}
		return null;
	}

	public void RemoveWeapon(string weaponName)
	{
		Weapon weapon = weapons.Find((Weapon w) => w.name == weaponName);
		if (weapon != null)
		{
			data.weapons.Remove(weapon.GetData());
			weapons.Remove(weapon);
			UnityEngine.Object.Destroy(weapon.gameObject);
			if (player.isMine)
			{
				player.game.netView.RPC("RPCRemoveWeapon", RPCMode.Others, player.GetNetworkPlayerID(), weaponName);
			}
		}
	}

	public void DropWeapon()
	{
		Weapon weapon = GetWeapon();
		if (weapon != null)
		{
			int bullets = weapon.GetBullets();
			int chargers = weapon.GetChargers();
			Transform child = weapon.transform.GetChild(0).GetChild(0);
			child.GetComponent<Collider>().enabled = true;
			child.GetComponent<Rigidbody>().useGravity = true;
			child.GetComponent<Rigidbody>().AddForce(base.transform.forward * 20f, ForceMode.Impulse);
			weapon.transform.parent = player.game.droppedWeapons;
			weapon.player = null;
			data.weapons.Remove(weapon.GetData());
			weapons.Remove(weapon);
			player.modelController.model.knife.gameObject.SetActive(true);
			player.animationController.SetAnimType(AnimationController.AnimType.Knife);
			data.slot = 0;
			if (player.isMine)
			{
				player.game.ui.hud.UpdateWeapons();
				player.game.netView.RPC("RPCDropWeapon", RPCMode.Others, player.GetNetworkPlayerID(), bullets, chargers);
			}
		}
		else
		{
			Debug.Log("No weapon attached");
		}
	}

	public Weapon GetWeapon()
	{
		return weapons.Find((Weapon w) => w.slot == (Slot)data.slot);
	}

	public Weapon GetWeapon(Slot slot)
	{
		return weapons.Find((Weapon w) => w.slot == slot);
	}

	public Weapon Find(string item)
	{
		return weapons.Find((Weapon w) => w.model.ToString() == item);
	}

	public InventoryData GetData()
	{
		return data;
	}

	public void SetData(InventoryData inventory)
	{
		data = inventory;
		foreach (Weapon.WeaponData weapon2 in data.weapons)
		{
			Weapon.Model type = (Weapon.Model)weapon2.type;
			string weaponName = type.ToString();
			Weapon weapon = AddWeapon(weaponName);
			weapon.SetData(weapon2);
		}
		Debug.Log("Inventory.SetData.SetSlot: " + GetSlot());
		SetSlot(GetSlot());
	}
}
