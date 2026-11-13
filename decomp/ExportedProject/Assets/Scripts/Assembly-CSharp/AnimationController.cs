using UnityEngine;

public class AnimationController : MonoBehaviour
{
	public enum AnimType
	{
		Knife = 0,
		Handgun = 1,
		Assault = 2,
		Rifle = 3,
		Machinegun = 4
	}

	public enum AnimState
	{
		Idle = 0,
		Run = 1,
		RunStrafeL = 2,
		RunStrafeR = 3,
		Back = 4,
		BackStrafeL = 5,
		BackStrafeR = 6,
		Shoot = 7,
		Reload = 8,
		KnifeA = 9,
		KnifeB = 10,
		He = 11,
		Die = 12,
		Celebrate1 = 13,
		Celebrate2 = 14
	}

	public Player player;

	public RuntimeAnimatorController rifleAnimatorController;

	public AnimatorOverrideController overrideAnimatorAssault;

	public AnimatorOverrideController overrideAnimatorHandgun;

	public AnimatorOverrideController overrideAnimatorKnife;

	public AnimatorOverrideController overrideAnimatorMachinegun;

	public AnimType animType = AnimType.Handgun;

	private AnimState animState;

	public Vector2 move;

	public RaycastHit hit;

	public AudioClip[] footSteps;

	public AudioSource audioSource;

	private void Update()
	{
		if (move.magnitude > 0f)
		{
			audioSource.volume = move.magnitude * 0.5f;
			audioSource.pitch = move.magnitude;
			if (!audioSource.isPlaying)
			{
				audioSource.clip = footSteps[Random.Range(0, footSteps.Length - 1)];
				audioSource.Play();
			}
		}
	}

	public void SetAnimType(Weapon w)
	{
		SetAnimType(w.animType);
	}

	public void SetAnimType(AnimType at)
	{
		animType = at;
		switch (at)
		{
		case AnimType.Knife:
			player.modelController.model.animator.runtimeAnimatorController = overrideAnimatorKnife;
			break;
		case AnimType.Rifle:
			player.modelController.model.animator.runtimeAnimatorController = rifleAnimatorController;
			break;
		case AnimType.Assault:
			player.modelController.model.animator.runtimeAnimatorController = overrideAnimatorAssault;
			break;
		default:
			player.modelController.model.animator.runtimeAnimatorController = overrideAnimatorHandgun;
			break;
		}
	}

	public bool IsPlayingHe()
	{
		return player.modelController.model.animator.GetCurrentAnimatorStateInfo(1).IsName("He");
	}

	public bool IsPlayingKnife()
	{
		return player.modelController.model.animator.GetCurrentAnimatorStateInfo(1).IsName("KnifeA") || player.modelController.model.animator.GetCurrentAnimatorStateInfo(1).IsName("KnifeB");
	}

	public bool SkipIK()
	{
		return IsPlayingHe() || player.modelController.model.animator.GetCurrentAnimatorStateInfo(1).IsName("Celebrate2");
	}

	public void Play(AnimState an)
	{
		animState = an;
		string stateName = animState.ToString();
		if (an == AnimState.Die)
		{
			player.animationController.OnMoveEnd();
			stateName = "Die" + Random.Range(1, 4);
		}
		player.modelController.model.animator.Play(stateName);
	}

	public void PlayReload(float duration)
	{
		if (!(player == null) && player.IsAlive())
		{
			animState = AnimState.Reload;
			if (animType == AnimType.Handgun)
			{
				player.modelController.model.animator.SetFloat("rspeed", duration * 1.5f);
			}
			else
			{
				player.modelController.model.animator.SetFloat("rspeed", duration * 2f);
			}
			player.modelController.model.animator.Play(animState.ToString());
		}
	}

	public void OnMove(Vector2 mv)
	{
		if (!(player == null) && player.IsAlive())
		{
			move = mv;
			player.modelController.model.animator.SetFloat("x", move.x);
			player.modelController.model.animator.SetFloat("y", move.y);
			player.modelController.model.animator.SetFloat("speed", move.magnitude);
		}
	}

	public void OnMoveEnd()
	{
		if (!(player == null))
		{
			move = Vector2.zero;
			player.modelController.model.animator.SetFloat("x", 0f);
			player.modelController.model.animator.SetFloat("y", 0f);
			player.modelController.model.animator.SetFloat("speed", 0f);
		}
	}

	public void OnAnimatorIKCB(int layerIndex)
	{
		if (player == null || !player.IsAlive() || SkipIK())
		{
			return;
		}
		if (player.isMine)
		{
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			int num = 17412;
			Physics.Raycast(ray, out hit, 1000f, ~num);
		}
		if (!(player.GetInventory() == null))
		{
			Weapon weapon = player.GetInventory().GetWeapon();
			if (weapon != null)
			{
				weapon.transform.LookAt(hit.point);
			}
			player.modelController.model.animator.SetLookAtWeight(1f, 1f, 0f, 0f, 1f);
			player.modelController.model.animator.SetLookAtPosition(hit.point);
		}
	}
}
