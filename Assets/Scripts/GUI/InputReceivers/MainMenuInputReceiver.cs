using UnityEngine;
using System.Collections;

public class MainMenuInputReceiver : InputReceiver
{
	CameraFade _cameraFade;
	
	public override void Start()
	{
		base.Start();
		
		_cameraFade = Camera.main.GetComponent<CameraFade>();		
		_cameraFade.OnFadeInCompleted += OnFadeInCompleted;
		_cameraFade.OnFadeOutCompleted += OnFadeOutCompleted;
	}
	
	void OnFadeInCompleted()
	{
		//
	}
	
	void OnFadeOutCompleted()
	{
		switch (cleanPressedName) 
        {
            case "PlayButton":
                {	
					Destroy(GameObject.Find("Sound Players"));
					Application.LoadLevel("Level_Scene"); // Loading_Scene

                    break;
                }

            case "HelpButton":
                {
                    Application.LoadLevel("HelpMenu_Scene");

                    break;
                }

            default:
                {
                    //

                    break;
                }
        }
	}
	
    public override void OnTouch(GUIElement guiTexture)
    {
		base.OnTouch(guiTexture);
		
		_cameraFade.FadeOut();
	}
}