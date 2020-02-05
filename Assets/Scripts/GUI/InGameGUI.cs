using UnityEngine;
using System;
using System.Collections;

public class InGameGUI : MonoBehaviour
{
	public GUIText scoreGUIText, highScoreGuIText;
	public GUITexture gameOverGUITexture, pauseGUITexture;
	
	string _scoreGUITextStart, _highScoreGuITextStart;
	GUITextShadow _scoreGUITextShadow, _highScoreGuITextShadow;
	
	void Awake() 
	{
		_scoreGUITextStart = scoreGUIText.text;
		_highScoreGuITextStart = highScoreGuIText.text;
		
		_scoreGUITextShadow = scoreGUIText.GetComponent<GUITextShadow>();
		_highScoreGuITextShadow = highScoreGuIText.GetComponent<GUITextShadow>();
	}

	void Update()
	{
		_scoreGUITextShadow.SetText(String.Format("{0} {1:00000}", _scoreGUITextStart, GameManager.instance.playerScore));
		_highScoreGuITextShadow.SetText(String.Format("{0} {1:00000}", _highScoreGuITextStart, GameManager.instance.playerHighScore));
		
		if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
            {
                Application.Quit();
                return;
            }
        }
	}
}