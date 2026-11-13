using UnityEngine;

public class MuzzleFlashAnimate : MonoBehaviour
{
	private Transform thisTransform;

	private Vector3 scale = Vector3.zero;

	private Vector3 angle = Vector3.zero;

	private void Awake()
	{
		thisTransform = base.transform;
		angle = thisTransform.localEulerAngles;
	}

	private void Update()
	{
		scale = Vector3.one * Random.Range(0.2f, 1f);
		thisTransform.localScale = scale;
		angle.z = Random.Range(0f, 90f);
		thisTransform.localEulerAngles = angle;
	}
}
