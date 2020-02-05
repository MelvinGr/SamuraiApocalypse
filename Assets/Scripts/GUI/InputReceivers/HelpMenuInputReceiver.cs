using UnityEngine;
using System.Collections;

public class HelpMenuInputReceiver : InputReceiver
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
            case "BackButton":
                {
                    //BloodSpatterManager.instance.OnSpatterCompleted += OnSpatterCompleted;
                    //BloodSpatterManager.instance.SpawnBloodSpatters(50, 0.01f);
                    Application.LoadLevel("MainMenu_Scene");
					
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