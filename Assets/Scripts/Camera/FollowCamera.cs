using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
	
	Vector2 _startOffset;
    public Vector2 offset;
    public float smoothTime = 0.3f;
	
	public bool slideIn = true;
	public Vector2 slideInOffset;
	
    Vector2 velocity;
	
	void Awake()
	{
		if(slideIn)
		{	
			_startOffset = offset;
			offset.x = slideInOffset.x;
		}
	}
		
    void Update()
    {
        if (target == null)
            return;
			
		if(slideIn && offset.x < _startOffset.x)
		{
			offset.x += (1f / 15f); // 1/15 pixels per fps dus
		}
		else if(slideIn && offset.x > _startOffset.x)
		{
			offset.x -= (1f / 15f); // 1/15 pixels per fps dus
		}

        Vector3 position = transform.position;
        position.x = Mathf.SmoothDamp(transform.position.x, target.position.x + offset.x, ref velocity.x, smoothTime);
        position.y = Mathf.SmoothDamp(transform.position.y, target.position.y + offset.y, ref velocity.y, smoothTime);

        transform.position = position;
    }
}