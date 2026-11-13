using System.Collections;
using UnityEngine;

public class DestroyDelayed : MonoBehaviour
{
	public float delay = 1f;

	private void Start()
	{
		StartCoroutine(DestroyDelay());
	}

	private IEnumerator DestroyDelay()
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}
}
