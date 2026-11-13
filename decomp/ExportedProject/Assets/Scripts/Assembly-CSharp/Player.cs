using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Serializable]
	public class PlayerData
	{
		public string name;

		public int team;

		public int modelType;

		public int life;

		public int shield;

		public int deaths;

		public int kills;

		public int ping;

		public string country;

		public Inventory.InventoryData inventory;

		public PlayerData(string playerName, string playerCountry)
		{
			name = playerName;
			country = playerCountry;
			inventory = new Inventory.InventoryData();
			life = 0;
		}

		public override string ToString()
		{
			string text = "Name: " + name + " (" + country + ")\n";
			string text2 = text;
			text = text2 + "Team: " + team + "\n";
			text2 = text;
			text = text2 + "Life: " + life + "\n";
			text2 = text;
			text = text2 + "Shield: " + shield + "\n";
			text2 = text;
			text = text2 + "Deaths: " + deaths + "\n";
			text2 = text;
			text = text2 + "Kills: " + kills + "\n";
			return text + inventory.ToString();
		}
	}

	public bool isMine;

	public GameLogic game;

	public PlayerController controller;

	public CharacterController characterController;

	public NetworkPlayer networkPlayer;

	public NetworkView netView;

	public PlayerData data;

	public GameObject deathEffect;

	public AnimationController animationController;

	public ModelController modelController;

	public ParticleSystem hitEffect;

	public ParticleSystem hitEffectBlood;

	private NetworkInterpolatedTransform networkInterpoTrans;

	private Inventory inventory;

	private void Update()
	{
		if (!isMine)
		{
			return;
		}
		Vector3 normalized = Camera.main.transform.forward.normalized;
		Debug.DrawRay(modelController.model.handL.position, normalized);
		Transform transform = animationController.hit.transform;
		if (transform != null)
		{
			HitBox component = transform.GetComponent<HitBox>();
			if (component != null)
			{
				game.ui.playerInfo.Show(this, component.player);
			}
			else
			{
				game.ui.playerInfo.Hide();
			}
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (!isMine)
		{
			return;
		}
		if (hit.transform.tag == "Grenade")
		{
			if (inventory.grenades <= 0)
			{
				inventory.grenades++;
				game.netView.RPC("RPCDisable", RPCMode.All, hit.transform.name);
			}
		}
		else
		{
			if (!(hit.transform.tag == "Weapon"))
			{
				return;
			}
			Weapon component = hit.transform.parent.parent.GetComponent<Weapon>();
			if (inventory.GetWeapon(component.slot) == null)
			{
				inventory.AddWeapon(hit.transform.parent.parent.name);
				if (component.slot == Inventory.Slot.Primary)
				{
					inventory.SetSlot(Inventory.Slot.Primary);
				}
				else if (component.slot == Inventory.Slot.Secondary && inventory.GetWeapon() == null)
				{
					inventory.SetSlot(Inventory.Slot.Secondary);
				}
			}
		}
	}

	public void Init(PlayerData playerData, NetworkPlayer nPlayer, NetworkViewID viewID)
	{
		data = playerData;
		base.name = data.name;
		Debug.Log(data.name + ".Init: " + data.ToString());
		Debug.Log(data.name + ".Init.Inventory: " + data.inventory.ToString());
		networkInterpoTrans = GetComponent<NetworkInterpolatedTransform>();
		netView = GetComponent<NetworkView>();
		netView.viewID = viewID;
		networkPlayer = nPlayer;
		inventory = GetComponent<Inventory>();
		inventory.SetPlayer(this);
		if (viewID.isMine)
		{
			isMine = true;
			controller = GameObject.Find("PlayerController").GetComponent<PlayerController>();
			controller.player = this;
			Utility.SetLayerRecursively(base.gameObject, 10);
		}
		else
		{
			modelController.SetModel((Model.ModelType)data.modelType);
			inventory.SetData(data.inventory);
			bool active = GetLife() > 0;
			characterController.enabled = active;
			networkInterpoTrans.enabled = active;
			modelController.gameObject.SetActive(active);
		}
	}

	public void SetName(string n)
	{
		base.name = n;
		if (isMine)
		{
			Preferences.SetPlayerName(n);
		}
	}

	public void ShootGrenade(NetworkViewID viewID)
	{
		inventory.grenades--;
		animationController.Play(AnimationController.AnimState.He);
		StartCoroutine(ShootGrenadeRoutine(viewID));
		if (isMine)
		{
			game.netView.RPC("RPCShootGrenade", RPCMode.Others, GetNetworkPlayerID(), viewID);
		}
	}

	private IEnumerator ShootGrenadeRoutine(NetworkViewID viewID)
	{
		yield return new WaitForSeconds(0.6f);
		GameObject obj = UnityEngine.Object.Instantiate(inventory.grenade, modelController.model.handL.position, Quaternion.identity);
		obj.GetComponent<NetworkView>().viewID = viewID;
		Explodable grenade = obj.GetComponent<Explodable>();
		grenade.SetPlayer(this);
		grenade.InitExplosion();
		if (isMine)
		{
			Collider component = obj.GetComponent<Collider>();
			Physics.IgnoreCollision(component, characterController);
			Collider[] colliders = modelController.model.colliders;
			foreach (Collider collider in colliders)
			{
				if (collider != null)
				{
					Physics.IgnoreCollision(component, collider);
				}
			}
			Vector3 normalized = Camera.main.transform.forward.normalized;
			normalized += Vector3.up / 2f;
			Vector3 force = normalized * grenade.speed;
			obj.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(obj.GetComponent<Rigidbody>());
			obj.GetComponent<SphereCollider>().enabled = false;
		}
	}

	public void ShootStart()
	{
		if (inventory.GetSlot() == Inventory.Slot.Knife)
		{
			if (!animationController.IsPlayingKnife())
			{
				AnimationController.AnimState animState = ((!((double)UnityEngine.Random.value > 0.5)) ? AnimationController.AnimState.KnifeB : AnimationController.AnimState.KnifeA);
				animationController.Play(animState);
				modelController.GetModel().knife.PlaySlash();
				if (isMine)
				{
					game.netView.RPC("RPCAnim", RPCMode.Others, GetNetworkPlayerID(), (int)animState);
				}
			}
		}
		else
		{
			Weapon weapon = inventory.GetWeapon();
			if (weapon != null)
			{
				weapon.ShootStart();
			}
		}
	}

	public void Shoot(Vector3 hitPoint)
	{
		Weapon weapon = inventory.GetWeapon();
		if (weapon != null)
		{
			Vector3 zero = Vector3.zero;
			zero = weapon.Shoot(hitPoint);
			if (isMine && zero != Vector3.zero)
			{
				game.ui.hud.UpdateBullets(weapon);
				game.netView.RPC("RPCShoot", RPCMode.Others, GetNetworkPlayerID(), zero);
			}
		}
	}

	public void ShootEnd()
	{
		Weapon weapon = inventory.GetWeapon();
		if (weapon != null)
		{
			weapon.ShootEnd();
		}
	}

	public Inventory GetInventory()
	{
		return inventory;
	}

	public NetworkPlayer GetNetworkPlayer()
	{
		return networkPlayer;
	}

	public int GetNetworkPlayerID()
	{
		return int.Parse(networkPlayer.ToString());
	}

	public NetworkViewID GetNetworkViewID()
	{
		return GetComponent<NetworkView>().viewID;
	}

	public GameLogic.Team GetTeam()
	{
		return (GameLogic.Team)data.team;
	}

	public void SetTeam(GameLogic.Team team, int modelType)
	{
		Debug.Log(base.name + ".SetTeam: " + team);
		data.modelType = modelType;
		data.team = (int)team;
		if (team != GameLogic.Team.Spectator)
		{
			modelController.SetModel((Model.ModelType)modelType);
		}
	}

	public int GetLife()
	{
		return data.life;
	}

	public void SetLife(int life)
	{
		data.life = life;
	}

	public int GetShield()
	{
		return data.shield;
	}

	public void SetShield(int shield)
	{
		data.shield = shield;
	}

	public int GetDeaths()
	{
		return data.deaths;
	}

	public void SetDeaths(int deaths)
	{
		data.deaths = deaths;
	}

	public void AddDeaths()
	{
		data.deaths++;
	}

	public int GetKills()
	{
		return data.kills;
	}

	public void SetKills(int kills)
	{
		data.kills = kills;
	}

	public void AddKills()
	{
		if (isMine)
		{
			game.ui.MessageTopLong(Ui.MessageType.Info, "+ 300$");
			game.cloud.AddKill();
		}
		inventory.SetMoney(inventory.GetMoney() + 300);
		data.kills++;
	}

	public void Die(bool dropWeapon, bool immediate = false)
	{
		if (isMine)
		{
			game.cloud.AddDeath();
		}
		if (dropWeapon)
		{
			inventory.DropWeapon();
			Weapon weapon = inventory.GetWeapon(Inventory.Slot.Primary);
			if (weapon != null)
			{
				inventory.RemoveWeapon(weapon.name);
			}
			weapon = inventory.GetWeapon(Inventory.Slot.Secondary);
			if (weapon != null)
			{
				inventory.RemoveWeapon(weapon.name);
			}
		}
		data.life = 0;
		data.shield = 0;
		data.deaths++;
		characterController.enabled = false;
		if (isMine)
		{
			controller.enabled = false;
			game.ui.Show(Ui.UiState.Spectator);
		}
		if (immediate)
		{
			Enable(false);
			return;
		}
		animationController.Play(AnimationController.AnimState.Die);
		StartCoroutine(DieRoutine());
	}

	private IEnumerator DieRoutine()
	{
		yield return new WaitForSeconds(2f);
		if (!IsAlive())
		{
			UnityEngine.Object.Instantiate(deathEffect, base.transform.position, Quaternion.identity);
			Enable(false);
		}
	}

	public void Reset(bool enable, bool resetMoney, bool resetInventory = true)
	{
		Debug.Log(base.name + ".Reset(" + enable + ", " + resetMoney + ")");
		if (enable && GetTeam() != GameLogic.Team.Spectator)
		{
			if (resetInventory)
			{
				inventory.Reset(resetMoney);
			}
			data.shield = ((data.life <= 20) ? 20 : data.shield);
			data.life = 100;
		}
		else
		{
			data.life = 0;
		}
		Enable(enable);
	}

	private void Enable(bool enable)
	{
		Debug.Log(base.name + ".Enable(" + enable + ")");
		if (enable)
		{
			StopCoroutine(DieRoutine());
		}
		Transform spawnRandom = game.config.maps.current.GetSpawnRandom(GetTeam());
		base.transform.position = spawnRandom.position;
		base.transform.eulerAngles = new Vector3(0f, spawnRandom.localEulerAngles.y, 0f);
		data.life = (enable ? 100 : 0);
		characterController.enabled = enable;
		if (enable)
		{
			animationController.Play(AnimationController.AnimState.Idle);
		}
		modelController.gameObject.SetActive(enable);
		if (isMine)
		{
			controller.enabled = enable;
			if (enable)
			{
				game.ui.Show(Ui.UiState.Hud);
			}
			else
			{
				game.ui.Show(Ui.UiState.Spectator);
			}
		}
		else
		{
			networkInterpoTrans.enabled = enable;
		}
	}

	public void SetCameraTarget(Player target)
	{
		if (isMine && GetLife() <= 0)
		{
			if (target == null || target.isMine)
			{
				target = game.players.GetPlayerRandom();
			}
			if (target != null)
			{
				controller.cameraController.SetPlayer(target);
				game.ui.menuDeadPlayerInfo.Show(this, target);
			}
			else
			{
				game.ui.menuDeadPlayerInfo.Hide();
			}
		}
	}

	public bool IsAlive()
	{
		return data.life > 0;
	}

	public int GetPing()
	{
		return data.ping;
	}

	public void SetPing(int ping)
	{
		data.ping = ping;
	}

	public PlayerData GetData()
	{
		return data;
	}
}
