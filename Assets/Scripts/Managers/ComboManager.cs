using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboManager : MonoBehaviour
{
	[System.Serializable]
	public class ComboTexture
	{
		public int ID;
		public Texture2D guiTexture;
		
		[HideInInspector]
		public Combo combo;
	}

    static ComboManager _instance = null;
    public static ComboManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (ComboManager)FindObjectOfType(typeof(ComboManager));

            return _instance;
        }
    }

    public delegate void ComboCompletedDelegate();
    public ComboCompletedDelegate OnComboCompleted;

    public ComboTexture[] comboTextures;

    GameObject _comboGameObjects;
	
	void Start()
	{
		OnLevelWasLoaded(Application.loadedLevel);
	}

    void OnLevelWasLoaded(int level)
	{
        if (level != GameManager.levelID || _comboGameObjects != null)
			return;
			
        _comboGameObjects = new GameObject("ComboGameObjects");
        _comboGameObjects.transform.position = Vector3.zero;
		
		if(GameObject.Find("InGameGUI_Prefab") != null)
			_comboGameObjects.transform.parent = GameObject.Find("InGameGUI_Prefab").transform;		

        foreach (ComboTexture comboTexture in comboTextures)
        {
            GameObject gameObject = new GameObject(comboTexture.guiTexture.name.Replace("_GUITexture", ""), typeof(GUITexture));
            gameObject.transform.localScale = new Vector3(0, 0, 1);
            gameObject.GetComponent<GUITexture>().texture = comboTexture.guiTexture;
            gameObject.GetComponent<GUITexture>().pixelInset = new Rect(-comboTexture.guiTexture.width / 2, -comboTexture.guiTexture.height, 
				comboTexture.guiTexture.width, comboTexture.guiTexture.height);
		
            gameObject.transform.parent = _comboGameObjects.transform;
            gameObject.active = false;

			comboTexture.combo = gameObject.AddComponent<Combo>();
        }
    }

    public void SpawnCombo(Vector2 position, float duration, int comboCount)
    {
		if(!enabled)
			return;
			
		foreach(ComboTexture comboTexure in comboTextures)
		{
			if(comboTexure.ID != comboCount)
				continue;
				
			if(comboTexure.combo.enabled)
				break;
				
			comboTexure.combo.SetActive(position, Time.time, duration);
		}
    }
}
