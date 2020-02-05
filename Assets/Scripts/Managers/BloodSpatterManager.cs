using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodSpatterManager : MonoBehaviour
{
    static BloodSpatterManager _instance = null;
    public static BloodSpatterManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (BloodSpatterManager)FindObjectOfType(typeof(BloodSpatterManager));

            return _instance;
        }
    }

    public delegate void SpatterCompletedDelegate();
    public SpatterCompletedDelegate OnSpatterCompleted;

    public Texture2D[] bloodSpatterTextures;

    GameObject _bloodSpatterGameObjects;
    List<BloodSpatter> _cachedBloodSpatters = new List<BloodSpatter>();

	void Start()
	{
		OnLevelWasLoaded(Application.loadedLevel);
	}
	
    void OnLevelWasLoaded(int level)
	{
		if (level != GameManager.levelID || _bloodSpatterGameObjects != null)
			return;
		
        _bloodSpatterGameObjects = new GameObject("BloodSpatterGameObjects");
        _bloodSpatterGameObjects.transform.position = Vector3.zero;
		
		if(GameObject.Find("InGameGUI_Prefab") != null)
			_bloodSpatterGameObjects.transform.parent = GameObject.Find("InGameGUI_Prefab").transform;		

        foreach (Texture2D bloodSpatterTexture in bloodSpatterTextures)
        {
            GameObject gameObject = new GameObject(bloodSpatterTexture.name.Replace("_GUITexture", "(Cached)"), typeof(GUITexture));
            gameObject.transform.localScale = new Vector3(0, 0, 1);
            gameObject.GetComponent<GUITexture>().texture = bloodSpatterTexture;
            gameObject.GetComponent<GUITexture>().pixelInset = new Rect(-bloodSpatterTexture.width / 2, -bloodSpatterTexture.height / 2,
                bloodSpatterTexture.width, bloodSpatterTexture.height);

            gameObject.transform.parent = _bloodSpatterGameObjects.transform;
            gameObject.active = false;

            _cachedBloodSpatters.Add(gameObject.AddComponent<BloodSpatter>());
        }
    }

    public void SpawnBloodSpatter(Vector2 position, float duration)
    {
		if(!enabled)
			return;
			
        BloodSpatter bloodSpatter = _cachedBloodSpatters[UnityEngine.Random.Range(0, _cachedBloodSpatters.Count)];
		if(bloodSpatter == null)
			return;
			
        if (bloodSpatter.gameObject.active) // cache is al actief, instantiate een nieuwe die later weer verwijderd word
        {
            bloodSpatter = ((GameObject)Instantiate(bloodSpatter.gameObject)).GetComponent<BloodSpatter>();
            bloodSpatter.transform.parent = _bloodSpatterGameObjects.transform;
            bloodSpatter.isClone = true;
        }

        bloodSpatter.SetActive(position, Time.time, duration);
    }

    public void SpawnBloodSpatters(int count, float pauseTime, float duration)
    {
		if(!enabled)
			return;
			
        StartCoroutine(_SpawnBloodSpatters(count, pauseTime, duration));
    }

    IEnumerator _SpawnBloodSpatters(int count, float pauseTime, float duration)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBloodSpatter(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value), duration);
            yield return new WaitForSeconds(pauseTime);
        }

        if (OnSpatterCompleted != null)
            OnSpatterCompleted();
    }
}
