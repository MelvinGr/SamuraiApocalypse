using UnityEngine;
using System.Collections;

public class DebugManager : MonoBehaviour
{
    static DebugManager _instance = null;
    public static DebugManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (DebugManager)FindObjectOfType(typeof(DebugManager));

            return _instance;
        }
    }

    public GUIText debugGUIText;
	public GUIText fpsGUIText;
	
	public float FPSupdateInterval = 0.5f;
    float _FPSaccum = 0;
    int _FPSframes = 0;
    float _FPStimeleft;
	
	void Awake()
	{
		if(!Debug.isDebugBuild)
			Destroy(gameObject);
	}
	
    void OnGUI()
    {
        string debugText = "";//Score: " + Player.instance.score + "\nKillCount: " + Player.instance.killCount + 
			//"\nLastKillTime: " + Player.instance.lastKillTime + "\nPreviousLastKillTime: " + Player.instance.previousLastKillTime;

        debugGUIText.text = debugText;
        debugGUIText.material.color = Color.blue;
    }
	
	void Update()
	{
		UpdateFPS();
		
		//
	}

    void UpdateFPS()
    {
        _FPStimeleft -= Time.deltaTime;
        _FPSaccum += Time.timeScale / Time.deltaTime;
        _FPSframes++;

        if (_FPStimeleft <= 0.0)
        {
            float fps = _FPSaccum / _FPSframes;
            fpsGUIText.text = System.String.Format("{0:F2} FPS", fps);

            if (fps < 10)
                fpsGUIText.material.color = Color.red;
			else if (fps < 30)
                fpsGUIText.material.color = Color.yellow;            
            else
                fpsGUIText.material.color = Color.green;

            _FPStimeleft = FPSupdateInterval;
            _FPSaccum = 0;
            _FPSframes = 0;
        }
    }
}