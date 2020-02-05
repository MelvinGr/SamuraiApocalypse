using UnityEngine;
using System;
using System.Collections;

public class InputManager : MonoBehaviour
{
    static InputManager _instance = null;
    public static InputManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (InputManager)FindObjectOfType(typeof(InputManager));

            return _instance;
        }
    }
	
	InputReceiver[] inputReceivers;
	
	void Start()
	{
		inputReceivers = (InputReceiver[])FindObjectsOfType(typeof(InputReceiver));	
	}
	
	void OnLevelWasLoaded(int level)
	{
		Start();
	}

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_NACL || UNITY_FLASH
	Vector2 _previousTouchPosition;
	float _previousTouchTime = 0;
#endif

    void Update()
    {			
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_NACL || UNITY_FLASH
		if(Input.GetMouseButtonDown(0))
		{					
			foreach(InputReceiver inputReceiver in inputReceivers)
			{			
				foreach(GUIElement guiElement in inputReceiver.guiElements)
				{				
					if(guiElement.HitTest(Input.mousePosition))
					{
						inputReceiver.OnTouch(guiElement);						
						break;
					}
				}
			}
		}	
#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchCount <= 0)
			return;
		
		for(int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if(touch.phase != TouchPhase.Began)
				continue;

			foreach(InputReceiver inputReceiver in inputReceivers)
			{			
				foreach(GUIElement guiElement in inputReceiver.guiElements)
				{				
					if(guiElement.HitTest(touch.position))
					{
						inputReceiver.OnTouch(guiElement);						
						break;
					}
				}
			}
		}
#else
        throw new System.ApplicationException("InputManager: " + Application.platform + " not supported!");
#endif
    }

    public int touchCount
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_NACL || UNITY_FLASH
			return (Input.GetMouseButtonDown(0) ? 1 : 0);
#elif UNITY_ANDROID || UNITY_IPHONE
			return Input.touchCount;
#else
            throw new System.ApplicationException("InputManager: " + Application.platform + " not supported!");
#endif
        }
    }

    public Touch GetTouch(int index)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_NACL || UNITY_FLASH	
		Vector2 touchPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		float touchTime = Time.time;		
		
		Touch touch = new Touch();
		Functions.SetPrivateValue(ref touch, "m_FingerId", 0);
		Functions.SetPrivateValue(ref touch, "m_Position", touchPosition);
		Functions.SetPrivateValue(ref touch, "m_DeltaPosition", _previousTouchPosition - touchPosition);
		Functions.SetPrivateValue(ref touch, "m_DeltaTime", touchTime - _previousTouchTime);
		Functions.SetPrivateValue(ref touch, "m_TapCount", 1);
		Functions.SetPrivateValue(ref touch, "m_Phase", TouchPhase.Began);
		
		_previousTouchPosition = touchPosition;
		_previousTouchTime = touchTime;
	
		return touch;
#elif UNITY_ANDROID || UNITY_IPHONE
		return Input.GetTouch(index);
#else
        throw new System.ApplicationException("InputManager: " + Application.platform + " not supported!");
#endif
    }
}
