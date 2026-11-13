using UnityEngine;

public class Knife : MonoBehaviour
{
	public Player player;

	public int power;

	public AudioSource audioSource;

	public AudioClip audioSlash1;

	public AudioClip audioSlash2;

	public AudioClip audioHit1;

	public AudioClip audioHit2;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		Utility.SetLayerRecursively(base.gameObject, 14);
	}

	public void PlaySlash()
	{
		if (Random.value > 0.5f)
		{
			audioSource.PlayOneShot(audioSlash1);
		}
		else
		{
			audioSource.PlayOneShot(audioSlash2);
		}
	}

	public void PlayHit(bool hitPlayer)
	{
		if (hitPlayer)
		{
			audioSource.PlayOneShot(audioHit1);
		}
		else
		{
			audioSource.PlayOneShot(audioHit2);
		}
	}

	public void CheckCollisions()
	{
		if (player == null)
		{
			Debug.LogWarning("CheckCollisions: player == null");
			return;
		}
		foreach (Player item in player.game.players.Get())
		{
			if (item.GetNetworkPlayerID() == player.GetNetworkPlayerID())
			{
				continue;
			}
			float num = Vector3.Distance(base.transform.position, item.transform.position);
			if (player.GetTeam() != item.GetTeam() && num < 1f)
			{
				player.hitEffectBlood.transform.position = base.transform.position;
				player.hitEffectBlood.transform.rotation = Quaternion.LookRotation(base.transform.position);
				player.hitEffectBlood.Play(true);
				PlayHit(true);
				if (player.isMine)
				{
					int num2 = power;
					Debug.Log("Knife: " + item.name + " | damage: " + num2);
					player.game.ui.MessageTopShort(Ui.MessageType.Damage, num2 + string.Empty);
					player.game.netView.RPC("RPCHit", RPCMode.All, player.GetNetworkPlayerID(), item.GetNetworkPlayerID(), num2);
				}
			}
		}
	}
}
