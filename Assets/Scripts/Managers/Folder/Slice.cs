using UnityEngine;
using System.Collections;

public class Slice
{
	public float startTime;
	public Texture2D texture;
	public Vector3 start;
	public Vector3 end;
	public float durationTime;

	public bool done = false;

	public Slice(Texture2D texture, Vector2 start, Vector2 end, float duration)
	{
		this.startTime = Time.time;
		this.texture = texture;
		this.start = start;
		this.end = end;
		this.durationTime = duration;
	}

	public void Draw()
	{
		if (done)
			return;

		float precentage = (Time.time - startTime) / durationTime;
		GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f - precentage);
		if (precentage < 1f)
			GUIHelper.DrawLineStretched(start, end, texture, texture.height);
		else
			done = true;
	}
};