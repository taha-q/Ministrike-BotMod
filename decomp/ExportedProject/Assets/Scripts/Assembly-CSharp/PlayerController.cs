using HedgehogTeam.EasyTouch;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Player player;

	public EasyTouch easyTouch;

	public ETCJoystick joystick;

	public ETCButton button;

	public UISlider swipeSensivitySlider;

	public UISlider swipeTolerenceSlider;

	public UISlider stationaryTolerenceSlider;

	public UISlider joystickSizeSlider;

	public UISlider buttonSizeSlider;

	public float forwardSpeed = 4f;

	public float backwardSpeed = 3f;

	public float sidestepSpeed = 4f;

	public float jumpSpeed = 8f;

	public float inAirMultiplier = 0.25f;

	public float pushPower = 2f;

	public CameraController cameraController;

	private Vector3 velocity;

	private bool[] cancelSwipe;

	private void Awake()
	{
		cancelSwipe = new bool[10];
		cameraController = GetComponent<CameraController>();
		easyTouch.enable = false;
		joystick.visible = false;
		button.visible = false;
	}

	private void Start()
	{
		ETCInput.SetControlSwipeIn("Joystick", true);
		float num = PlayerPrefs.GetFloat("swipeSensivity", 0.5f);
		swipeSensivitySlider.Set(num);
		OnSwipeSensivitySlider(num);
		num = PlayerPrefs.GetFloat("swipeTolerance", 0f);
		swipeTolerenceSlider.Set(num);
		OnSwipeToleranceSlider(num);
		num = PlayerPrefs.GetFloat("stationaryTolerance", 0f);
		stationaryTolerenceSlider.Set(num);
		OnStationaryToleranceSlider(num);
		num = PlayerPrefs.GetFloat("joystickSize", 0.5f);
		joystickSizeSlider.Set(num);
		OnJoystickSizeSlider(num);
		num = PlayerPrefs.GetFloat("buttonSize", 0.5f);
		buttonSizeSlider.Set(num);
		OnButtonSizeSlider(num);
	}

	public void Show(bool show)
	{
		easyTouch.enable = show;
		joystick.visible = show;
		button.visible = show;
	}

	public void OnSwipeSensivitySlider(float value)
	{
		PlayerPrefs.SetFloat("swipeSensivity", value);
		cameraController.sensitivity = (value * 10f + 1f) * 3f;
	}

	public void OnSwipeToleranceSlider(float value)
	{
		PlayerPrefs.SetFloat("swipeTolerance", value);
		EasyTouch.SetSwipeTolerance(value);
	}

	public void OnStationaryToleranceSlider(float value)
	{
		PlayerPrefs.SetFloat("stationaryTolerance", value);
		EasyTouch.SetStationaryTolerance(value * 10f);
	}

	public void OnJoystickSizeSlider(float value)
	{
		PlayerPrefs.SetFloat("joystickSize", value);
		float num = 80f + value * 150f;
		joystick.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num);
		joystick.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(num * 0.6f, num * 0.6f);
	}

	public void OnButtonSizeSlider(float value)
	{
		PlayerPrefs.SetFloat("buttonSize", value);
		float num = 50f + value * 100f;
		button.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num);
	}

	public void OnButtonDown()
	{
		if (!(player == null) && player.IsAlive())
		{
			player.ShootStart();
		}
	}

	public void OnButtonPress()
	{
		if (!(player == null) && player.IsAlive())
		{
			player.Shoot(Vector3.zero);
		}
	}

	public void OnButtonUp()
	{
		if (!(player == null) && player.IsAlive())
		{
			player.ShootEnd();
		}
	}

	public void OnJoystickMoveStart()
	{
	}

	public void OnJoystickMove(Vector2 move)
	{
		if (!(player == null) && player.IsAlive())
		{
			Move(move);
			player.animationController.OnMove(move);
		}
	}

	public void OnJoystickMoveEnd()
	{
		if (!(player == null))
		{
			player.characterController.Move(Vector3.zero);
			player.animationController.OnMoveEnd();
		}
	}

	public void OnSwipeStart(Gesture gesture)
	{
		cancelSwipe[gesture.fingerIndex] = gesture.IsOverRectTransform(joystick.rectTransform());
	}

	public void OnSwipe(Gesture gesture)
	{
		if (!cancelSwipe[gesture.fingerIndex])
		{
			cameraController.Apply(gesture.deltaPosition);
		}
	}

	private void Move(Vector2 axis)
	{
		Vector3 motion = player.transform.TransformDirection(new Vector3(axis.x, 0f, axis.y));
		motion.y = 0f;
		motion.Normalize();
		Vector2 vector = new Vector2(Mathf.Abs(axis.x), Mathf.Abs(axis.y));
		if (vector.y > vector.x)
		{
			if (axis.y > 0f)
			{
				motion *= forwardSpeed * vector.y;
			}
			else
			{
				motion *= backwardSpeed * vector.y;
			}
		}
		else
		{
			motion *= sidestepSpeed * vector.x;
		}
		if (!player.characterController.isGrounded)
		{
			velocity.y += Physics.gravity.y * Time.deltaTime;
			motion.x *= inAirMultiplier;
			motion.z *= inAirMultiplier;
		}
		motion += velocity;
		motion += Physics.gravity;
		motion *= Time.deltaTime;
		player.characterController.Move(motion);
		if (player.characterController.isGrounded)
		{
			velocity = Vector3.zero;
		}
	}
}
