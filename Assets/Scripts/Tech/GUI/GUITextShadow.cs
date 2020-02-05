using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class GUITextShadow : MonoBehaviour 
{
	public Font shadowFont;
	public Vector2 shadowOffset = new Vector2(0, -0.004999995f);
	
	GUIText shadowGuIText;

	void Start() 
	{
		if(transform.parent.GetComponent<GUITextShadow>() != null)
		{		
#if UNITY_EDITOR
			DestroyImmediate(this);
#else
			Destroy(this);
#endif
			return;
		}
	
		GameObject shadow = (GameObject)Instantiate(gameObject);		
		shadow.transform.parent = transform;
		shadow.transform.localPosition = new Vector3(shadowOffset.x, shadowOffset.y, transform.localPosition.z - 1);
		
		shadowGuIText = shadow.GetComponent<GUIText>();
		shadowGuIText.font = shadowFont;
	}
	
	public void SetText(string text)
	{
		guiText.text = text;
		shadowGuIText.text = text;
	}
}
