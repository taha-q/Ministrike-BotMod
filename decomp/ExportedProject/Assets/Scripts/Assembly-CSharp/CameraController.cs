using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float initialDistance = 3f;

	public float minDistance = 3f;

	public float maxDistance = 5f;

	public bool clampPitchAngle = true;

	public float minPitch = -35f;

	public float maxPitch = 35f;

	public float smoothZoomSpeed = 6f;

	public float smoothOrbitSpeed = 6f;

	public float sensitivity = 15f;

	private float distance = 10f;

	private float yaw;

	private float pitch;

	private float idealDistance;

	private float idealYaw;

	private float idealPitch;

	private Player player;

	private Transform target;

	private Transform cameraTrans;

	private Transform thisTransform;

	public float Distance
	{
		get
		{
			return distance;
		}
	}

	public float IdealDistance
	{
		get
		{
			return idealDistance;
		}
		set
		{
			idealDistance = Mathf.Clamp(value, minDistance, maxDistance);
		}
	}

	public float Yaw
	{
		get
		{
			return yaw;
		}
	}

	public float IdealYaw
	{
		get
		{
			return idealYaw;
		}
		set
		{
			idealYaw = value;
		}
	}

	public float Pitch
	{
		get
		{
			return pitch;
		}
	}

	public float IdealPitch
	{
		get
		{
			return idealPitch;
		}
		set
		{
			idealPitch = ((!clampPitchAngle) ? value : ClampAngle(value, minPitch, maxPitch));
		}
	}

	private void Awake()
	{
		thisTransform = base.transform;
		cameraTrans = Camera.main.transform;
	}

	public void SetPlayer(Player p)
	{
		if (!(player != null) || p.GetNetworkPlayerID() != player.GetNetworkPlayerID())
		{
			player = p;
			target = p.transform.FindChild("CameraTarget");
			float num = (IdealDistance = initialDistance);
			distance = num;
			num = (IdealYaw = 0f);
			yaw = num;
			num = (IdealPitch = 0f);
			pitch = num;
			Apply();
		}
	}

	public Player GetTargetPlayer()
	{
		return player;
	}

	public void PitchUp(float v)
	{
		IdealPitch -= v;
	}

	public void Apply(Vector2 delta)
	{
		float num = sensitivity * (Camera.main.fieldOfView / 60f);
		IdealYaw += delta.x * num * 0.02f;
		IdealPitch -= delta.y * num * 0.02f;
	}

	private void LateUpdate()
	{
		Apply();
	}

	private void Apply()
	{
		if ((bool)target)
		{
			int num = 19972;
			RaycastHit hitInfo = default(RaycastHit);
			Vector3 start = player.transform.position + Vector3.up;
			Vector3 end = player.transform.position + Vector3.up + -player.transform.forward * maxDistance;
			if (Physics.Linecast(start, end, out hitInfo, ~num))
			{
				Debug.DrawLine(start, hitInfo.point);
				idealDistance = Mathf.Clamp(hitInfo.distance - 0.8f, 0.3f, idealDistance);
			}
			hitInfo = default(RaycastHit);
			start = player.transform.position + Vector3.up;
			end = player.transform.position + Vector3.up + player.transform.up * maxDistance;
			if (Physics.Linecast(start, end, out hitInfo, ~num))
			{
				Debug.DrawLine(start, hitInfo.point);
				idealDistance = Mathf.Clamp(hitInfo.distance - 0.5f, 0.3f, idealDistance);
			}
			Vector3 localPosition = new Vector3(0.5f, 1.4f, 0f);
			localPosition.x = Mathf.Lerp(target.localPosition.x, 0.5f, Time.deltaTime * smoothOrbitSpeed);
			hitInfo = default(RaycastHit);
			start = player.transform.position + Vector3.up;
			end = player.transform.position + Vector3.up + player.transform.right * maxDistance;
			if (Physics.Linecast(start, end, out hitInfo, ~num))
			{
				Debug.DrawLine(start, hitInfo.point);
				float b = Mathf.Min(hitInfo.distance - 0.75f, 0.5f);
				localPosition.x = Mathf.Lerp(target.localPosition.x, b, Time.deltaTime * smoothOrbitSpeed * 2f);
			}
			hitInfo = default(RaycastHit);
			start = cameraTrans.position - cameraTrans.right / 2f;
			end = start + cameraTrans.right * 2f;
			if (Physics.Linecast(start, end, out hitInfo, ~num))
			{
				Debug.DrawLine(start, hitInfo.point);
				idealDistance = Mathf.Clamp(hitInfo.distance - 0.25f, 0.3f, idealDistance);
			}
			target.localPosition = localPosition;
			distance = Mathf.Lerp(distance, IdealDistance, Time.deltaTime * smoothZoomSpeed);
			yaw = Mathf.Lerp(yaw, IdealYaw, Time.deltaTime * smoothOrbitSpeed);
			pitch = Mathf.LerpAngle(pitch, IdealPitch, Time.deltaTime * smoothOrbitSpeed);
			thisTransform.position = target.position - distance * thisTransform.forward;
			if (player.isMine)
			{
				thisTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
				player.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
				float num2 = Mathf.Clamp(pitch, 0f, maxPitch);
				float num3 = num2 / maxPitch;
				IdealDistance = minDistance + (maxDistance - minDistance) * num3;
			}
			else
			{
				Quaternion b2 = Quaternion.LookRotation(player.animationController.hit.point - base.transform.position);
				thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, b2, Time.deltaTime * smoothOrbitSpeed);
				IdealDistance = 2f;
			}
		}
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
