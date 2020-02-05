using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

public class GameManager : MonoBehaviour
{
    static GameManager _instance = null;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (GameManager)FindObjectOfType(typeof(GameManager));

            return _instance;
        }
    }

    [HideInInspector]
    public Level _level;
    public static Level level
    {
        get { return instance._level; }
        set { instance._level = value; }
    }
	
	public int playerScore, playerHighScore, playerkillCount;
	
	public static int levelID = 2;
	
    void Awake()
    {
		UnityEngine.Random.seed = (int)(Environment.TickCount & ((int)(Input.acceleration.x + Input.acceleration.z) >> (int)Input.acceleration.y));

		playerHighScore = PlayerPrefs.GetInt("PlayerHighScore");
		
/*#if UNITY_EDITOR
		OnLevelWasLoaded(Application.loadedLevel);
#endif*/
    }
	
/*#if UNITY_EDITOR	
	void OnLevelWasLoaded(int level)
	{
		if(Application.loadedLevelName == "Level_Scene")
			levelID = 2;
		
		if(Application.loadedLevelName == "Test_Scene")
			levelID = 4;
	}
#endif*/

	public bool isPaused { get; private set; }
	public void Pause()
	{
		isPaused = !isPaused;
		
		if(Player.instance != null)
			Player.instance.Pause(isPaused);	
	}
	
    void Update()
    {
        //
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("PlayerHighScore", playerHighScore);

        _instance = null;
    }
}