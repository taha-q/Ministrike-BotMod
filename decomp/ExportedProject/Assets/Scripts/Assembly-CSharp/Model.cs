using UnityEngine;

public class Model : MonoBehaviour
{
	public enum ModelType
	{
		CTA = 0,
		CTB = 1,
		TEA = 2,
		TEB = 3
	}

	public AnimationController animationController;

	public Animator animator;

	public Knife knife;

	public Transform handR;

	public Transform handL;

	public Collider[] colliders;

	public ModelType type;

	private void Start()
	{
		colliders = GetComponentsInChildren<Collider>(true);
	}

	private void OnKnifeEvent()
	{
		knife.CheckCollisions();
	}

	private void OnAnimatorIK(int layerIndex)
	{
		animationController.OnAnimatorIKCB(layerIndex);
	}
}
