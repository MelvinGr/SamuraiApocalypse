using UnityEngine;
using System.Collections;

public class LevelInputReceiver : InputReceiver
{	
	public Texture2D pauseButtonTexture, playbuttonTexture;
	
	InGameGUI _inGameGUI;
	
	public override void Start()
	{
		base.Start();
		
		_inGameGUI = transform.parent.GetComponent<InGameGUI>();
	}
	
    public override void OnTouch(GUIElement guiElement)
    {
		base.OnTouch(guiElement);
		
		GUITexture _pauseGUITexture = (GUITexture)guiElement; 		
		switch (cleanPressedName) 
        {
            case "PlayButton":
                {	
					if(_pauseGUITexture.texture != playbuttonTexture)
					{	
						//Time.timeScale = 0;
						_pauseGUITexture.texture = playbuttonTexture;
						_inGameGUI.pauseGUITexture.active = false;
						GameManager.instance.Pause();
					}
					else
					{	
						//Player.instance.speed = 1;
						_pauseGUITexture.texture = pauseButtonTexture;
						_inGameGUI.pauseGUITexture.active = true;
						GameManager.instance.Pause();
					}
					
                    break;
                }

            default:
                {
                    //

                    break;
                }
        }
	}
}