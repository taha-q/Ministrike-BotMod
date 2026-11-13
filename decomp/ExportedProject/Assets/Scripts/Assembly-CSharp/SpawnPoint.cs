using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public enum Team
	{
		Spectator = 0,
		Counter = 1,
		Terro = 2
	}

	public Team team;

	private void OnDrawGizmos()
	{
		Gizmos.color = ((team != Team.Terro) ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f));
		Gizmos.DrawCube(base.transform.position, new Vector3(1f, 1f, 1f));
	}
}
