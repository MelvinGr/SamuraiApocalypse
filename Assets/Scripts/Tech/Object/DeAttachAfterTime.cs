using UnityEngine;
using System.Collections;

public class DeAttachAfterTime : MonoBehaviour
{
	public Transform targetTransform;	
	public float time;
	
	bool _started = false;
	
	public void StartTimer(float time)
	{
		this.time = time;
		_started = true;
	}

	void Update() 
	{
		if(gameObject.active && _started)
			time -= Time.deltaTime;
			
		if(time <= 0)
		{
			gameObject.transform.parent = targetTransform;
			_started = false;
		}
	}
}
