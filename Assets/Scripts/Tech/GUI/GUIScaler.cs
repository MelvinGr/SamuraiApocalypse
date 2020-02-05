using UnityEngine;
using System.Collections;

public class GUIScaler : MonoBehaviour
{
    public Vector2 originalScale = new Vector2(1280, 800);
	
    void Start()
	{
		foreach(GUITexture guiTexture in gameObject.GetComponentsInChildren<GUITexture>())
		{
			Rect pixelInset = guiTexture.pixelInset;
			pixelInset.width *= (Screen.width / originalScale.x);
			pixelInset.height *= (Screen.height / originalScale.y);
			guiTexture.pixelInset = pixelInset;
		}
	}
}