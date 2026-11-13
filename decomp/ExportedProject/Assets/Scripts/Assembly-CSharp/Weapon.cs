using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Weapon : MonoBehaviour
{
	public enum Model
	{
		Glock = 0,
		Deagle = 1,
		AK47 = 2,
		M4A1 = 3,
		MAC10 = 4,
		Smg = 5,
		AWP = 6,
		Shotgun = 7,
		Grenade = 8
	}

	[Serializable]
	public class WeaponData
	{
		public int type;

		public int chargers;

		public int bullets;
	}

	public AnimationController.AnimType animType;

	public Player player;

	public Model model;

	public Inventory.Slot slot;

	public GameObject muzzleFlash;

	public Material lineMaterial;

	public AudioClip audioFire;

	public AudioClip audioClipIn;

	public AudioClip audioClipOut;

	public AudioClip audioEmpty;

	private AudioSource audioSource;

	public int price;

	public int bulletsStock = 30;

	public int chargersStock = 3;

	public float rpm = 685f;

	public float precisionAngle = 1.5f;

	public float precisionRunningFactor = 3f;

	public float recoil = 1f;

	public float reloadTime = 1.5f;

	public int bulletDamage = 10;

	[Header("damage = damage - (distance * bulletDamageOverDistance)")]
	public float bulletDamageOverDistance = 1f;

	public int bulletsPerFire = 1;

	public Vector3 modelPos = Vector3.zero;

	public WeaponData data;

	private float nextFireTime;

	private VectorLine[] lineRay;

	private int fireCount;

	private float fireStartPitch;

	private float firePitch;

	private void Start()
	{
		lineRay = new VectorLine[bulletsPerFire];
		for (int i = 0; i < bulletsPerFire; i++)
		{
			lineRay[i] = new VectorLine("LineRay" + i, new List<Vector3>(2), 1f);
			if (lineMaterial != null)
			{
				lineRay[i].material = lineMaterial;
			}
			lineRay[i].SetColor(Color.yellow);
		}
		audioSource = GetComponent<AudioSource>();
		base.transform.GetChild(0).localPosition = modelPos;
		muzzleFlash.SetActive(false);
	}

	private void OnDestroy()
	{
		VectorLine.Destroy(lineRay);
	}

	private void Update()
	{
		base.transform.GetChild(0).localPosition = modelPos;
	}

	public void SetHand(Transform hand)
	{
		base.transform.parent = hand;
		base.transform.position = base.transform.parent.position;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.GetChild(0).GetChild(0).localPosition = Vector3.zero;
		base.transform.GetChild(0).GetChild(0).localRotation = Quaternion.identity;
	}

	public void Recharge()
	{
		data.bullets = bulletsStock;
		data.chargers = chargersStock;
	}

	public int GetBullets()
	{
		return data.bullets;
	}

	public void SetBullets(int bullets)
	{
		data.bullets = bullets;
	}

	public int GetChargers()
	{
		return data.chargers;
	}

	public void SetChargers(int chargers)
	{
		data.chargers = chargers;
	}

	public void Reload()
	{
		if (data.chargers > 0)
		{
			nextFireTime = Time.time + reloadTime * 10f;
			data.bullets = bulletsStock;
			data.chargers--;
			player.animationController.PlayReload(reloadTime);
			StartCoroutine(AudioReload());
		}
	}

	private IEnumerator AudioReload()
	{
		if (!audioClipOut && (bool)audioClipIn)
		{
			audioSource.clip = audioClipIn;
			audioSource.Play();
			yield return new WaitForSeconds(reloadTime);
		}
		else
		{
			audioSource.clip = audioClipOut;
			audioSource.Play();
			yield return new WaitForSeconds(reloadTime / 2f);
			if ((bool)audioClipIn)
			{
				audioSource.clip = audioClipIn;
				audioSource.Play();
			}
			yield return new WaitForSeconds(reloadTime / 2f);
		}
		nextFireTime = Time.time;
	}

	public void PlayEmpty()
	{
		if (!audioSource.isPlaying && (bool)audioEmpty)
		{
			audioSource.clip = audioEmpty;
			audioSource.Play();
		}
	}

	public void ShootStart()
	{
		fireCount = 0;
		if (player.isMine)
		{
			fireStartPitch = (firePitch = player.controller.cameraController.IdealPitch);
		}
	}

	public Vector3 Shoot(Vector3 hitPoint)
	{
		if (!player.isMine)
		{
			if (hitPoint == Vector3.up)
			{
				Reload();
				return Vector3.zero;
			}
			if (hitPoint == Vector3.down)
			{
				PlayEmpty();
				return Vector3.zero;
			}
			return ShootBullet(hitPoint);
		}
		if (Time.time > nextFireTime)
		{
			if (data.bullets == 0)
			{
				if (data.chargers == 0)
				{
					MonoBehaviour.print("no more bullet");
					PlayEmpty();
					return Vector3.down;
				}
				Reload();
				return Vector3.up;
			}
			return ShootBullet(hitPoint);
		}
		return Vector3.zero;
	}

	public Vector3 ShootBullet(Vector3 hitPoint)
	{
		Vector3 result = Vector3.zero;
		this.player.animationController.Play(AnimationController.AnimState.Shoot);
		if ((bool)audioFire)
		{
			audioSource.clip = audioFire;
			audioSource.Play();
		}
		for (int i = 0; i < bulletsPerFire; i++)
		{
			float num = this.player.characterController.velocity.magnitude * precisionRunningFactor;
			float num2 = precisionAngle + num;
			Vector3 vector = new Vector3(UnityEngine.Random.Range(0f - num2, num2), UnityEngine.Random.Range(0f - num2, num2), UnityEngine.Random.Range(0f - num2, num2));
			if (model != Model.Shotgun && model != Model.Glock && model != Model.Deagle && model != Model.AWP)
			{
				vector *= (float)((!(num > 0f)) ? Mathf.Max(1, fireCount) : fireCount);
			}
			Vector3 direction = ((!this.player.isMine) ? ((hitPoint - muzzleFlash.transform.position).normalized + ((i <= 0) ? Vector3.zero : (vector / 100f))) : ((this.player.animationController.hit.point - muzzleFlash.transform.position).normalized + vector / 100f));
			RaycastHit hitInfo = default(RaycastHit);
			int num3 = ((!this.player.isMine) ? 4 : 1028);
			if (!Physics.Raycast(muzzleFlash.transform.position, direction, out hitInfo, 1000f, ~num3))
			{
				continue;
			}
			result = ((!this.player.isMine) ? Vector3.zero : hitInfo.point);
			if (lineRay.Length > i)
			{
				lineRay[i].active = true;
				lineRay[i].points3[0] = muzzleFlash.transform.position + muzzleFlash.transform.forward / 4f;
				lineRay[i].points3[1] = hitInfo.point;
				lineRay[i].Draw3D();
			}
			switch (hitInfo.collider.gameObject.layer)
			{
			case 11:
			{
				Explodable component = hitInfo.collider.GetComponent<Explodable>();
				if ((bool)component)
				{
					component.Explode(this.player);
				}
				break;
			}
			case 9:
			case 10:
			{
				HitBox component2 = hitInfo.collider.GetComponent<HitBox>();
				if (!(component2 != null))
				{
					break;
				}
				Player player = component2.player;
				if (this.player.GetTeam() == player.GetTeam())
				{
					break;
				}
				this.player.hitEffectBlood.transform.position = hitInfo.point;
				this.player.hitEffectBlood.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
				this.player.hitEffectBlood.Play(true);
				if (this.player.isMine && player.IsAlive())
				{
					int num4 = bulletDamage;
					if (component2.type == HitBox.Type.Torse)
					{
						num4 = bulletDamage / 2;
					}
					else if (component2.type != HitBox.Type.Head)
					{
						num4 = bulletDamage / 3;
					}
					num4 = Mathf.Max(1, num4 - (int)(hitInfo.distance * bulletDamageOverDistance));
					this.player.game.ui.MessageTopShort(Ui.MessageType.Damage, -num4 + " (" + player.name + " @ " + component2.type.ToString() + ")");
					this.player.game.netView.RPC("RPCHit", RPCMode.All, this.player.GetNetworkPlayerID(), player.GetNetworkPlayerID(), num4);
				}
				break;
			}
			default:
				if (hitInfo.collider.tag != "Bounds")
				{
					this.player.hitEffect.transform.position = hitInfo.point;
					this.player.hitEffect.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
					this.player.hitEffect.Play(true);
				}
				break;
			}
		}
		nextFireTime = Time.time + 60f / rpm;
		muzzleFlash.SetActive(true);
		data.bullets--;
		fireCount++;
		if (this.player.isMine)
		{
			firePitch = this.player.controller.cameraController.Pitch;
			this.player.controller.cameraController.PitchUp(recoil * (float)fireCount);
		}
		StartCoroutine(DisableLineRay());
		return result;
	}

	private IEnumerator DisableLineRay()
	{
		yield return null;
		yield return null;
		for (int i = 0; i < lineRay.Length; i++)
		{
			lineRay[i].active = false;
		}
		muzzleFlash.SetActive(false);
		if (player.isMine)
		{
			player.controller.cameraController.IdealPitch = firePitch;
		}
	}

	public void ShootEnd()
	{
		fireCount = 0;
		if (player.isMine)
		{
			player.controller.cameraController.IdealPitch = (firePitch = fireStartPitch);
		}
	}

	public void SetData(WeaponData weaponData)
	{
		data = weaponData;
		data.type = (int)model;
	}

	public WeaponData GetData()
	{
		data.type = (int)model;
		return data;
	}
}
