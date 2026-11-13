using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour
{
	public enum Type
	{
		Head = 0,
		Torse = 1,
		LegL = 2,
		LegR = 3,
		ArmL = 4,
		ArmR = 5
	}

	public Player player;

	public Type type;
}
