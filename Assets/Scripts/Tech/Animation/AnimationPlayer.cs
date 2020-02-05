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
		try { bool t = (animation[animationName].name == name); }
		catch { return; }
		
		if(playSingle)
		{
			if(!_didPlay)
			{		
				animation.CrossFade(animationName);
				_didPlay = true;
			}
		}
		else
			animation.CrossFade(animationName);
    }
}
