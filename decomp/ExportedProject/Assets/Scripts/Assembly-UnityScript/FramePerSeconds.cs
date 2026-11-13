using System;
using UnityEngine;

[Serializable]
public class FramePerSeconds : MonoBehaviour
{
	public float updateInterval;

	private double lastInterval;

	private int frames;

	private float fps;

	public FramePerSeconds()
	{
		updateInterval = 0.5f;
	}

	public virtual void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}

	public virtual void OnGUI()
	{
		GUI.Label(new Rect(Screen.width - 60, 10f, 60f, 20f), "fps: " + fps.ToString("f0"));
	}

	public virtual void Update()
	{
		frames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!((double)realtimeSinceStartup <= lastInterval + (double)updateInterval))
		{
			fps = (float)((double)frames / ((double)realtimeSinceStartup - lastInterval));
			frames = 0;
			lastInterval = realtimeSinceStartup;
		}
	}

	public virtual void Main()
	{
	}
}
