using UnityEngine;
using System.Collections;

public class AnimationPlayer : MonoBehaviour
{
    public string animationName;
	
	public bool playSingle = false;
	bool _didPlay = false;
	
    void Update()
    {
		// check om te kijken of de animatie wel bestaat, MOET BETER!
		try { bool t = (GetComponent<Animation>()[animationName].name == name); }
		catch { return; }
		
		if(playSingle)
		{
			if(!_didPlay)
			{		
				GetComponent<Animation>().CrossFade(animationName);
				_didPlay = true;
			}
		}
		else
			GetComponent<Animation>().CrossFade(animationName);
    }
}
