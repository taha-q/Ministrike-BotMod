using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UITitle : MonoBehaviour
{
	[Serializable]
	public class TitleModel
	{
		public Animator animator;

		public AudioSource weaponAudio;

		public GameObject muzzleFlash;

		public TweenRotation tweener;

		public void Fire()
		{
			animator.Play("Shoot");
			weaponAudio.Play();
			muzzleFlash.SetActive(true);
		}

		public void FireEnd()
		{
			muzzleFlash.SetActive(false);
		}
	}

	public List<TitleModel> models;

	public UIMessageBox mbox;

	public bool disableBatchMode;

	private void Start()
	{
		StartCoroutine(UpdateRoutine());
	}

	private IEnumerator UpdateRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(2, 10));
			int fireCount = 0;
			int i = UnityEngine.Random.Range(0, models.Count);
			models[i].tweener.Play((UnityEngine.Random.value > 0.5f) ? true : false);
			while (fireCount < UnityEngine.Random.Range(1, 10))
			{
				models[i].Fire();
				yield return new WaitForSeconds(0.05f);
				models[i].FireEnd();
				fireCount++;
				yield return new WaitForSeconds(0.06f);
			}
			models[i].tweener.enabled = false;
		}
	}

	public void OnLoadSceneTitle()
	{
		SceneManager.LoadScene(0);
	}

	public void OnButtonPlayOnline()
	{
		Config.isLocal = false;
		SceneManager.LoadScene(1);
	}

	public void OnButtonPlayLocal()
	{
		Config.isLocal = true;
		SceneManager.LoadScene(1);
	}

	public void OnQuit()
	{
		Application.Quit();
	}
}
