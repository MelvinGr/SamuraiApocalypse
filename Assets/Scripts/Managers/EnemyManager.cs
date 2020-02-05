using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    static EnemyManager _instance = null;
    public static EnemyManager instance 
    {
        get
        {
            if (_instance == null)
                _instance = (EnemyManager)FindObjectOfType(typeof(EnemyManager));
			
            return _instance;
        }
    }

    public Transform[] enemyPrefabs, bossPrefabs;
	
	public Transform deadEnemies;

	//float _oldEnemieSpawnPauseTime;
    //public float enemieSpawnPauseTime = 1; 
	
	public int bossSpawnRate = 15;

    public float enemySpawnOffset = 10;
    public float enemyDestroyOffset = 15;
	public float enemySpawnDistance = 3;

    int _enemiesSpawned, _bossesSpawned;
    Vector3 _lastSpawnPosition = new Vector3();
	
	ZombieBoss _zombieBoss;
	
	int _introIndex = 0;
	public int[] introArray = new int[] { 0, 0, 1, 0, 0, 1, 1 };
	
	bool _bossAlive
	{
		get { return _zombieBoss != null; }
	}
	
	bool _bossActive = false;	
	bool _bossGameObjectActive
	{
		set { if(_zombieBoss != null) _zombieBoss.gameObject.SetActiveRecursively(value); }
	}

    void Awake()
    {
        if(deadEnemies == null)
		{
			deadEnemies = new GameObject("Dead Enemies").transform;
			deadEnemies.parent = transform;
		}
    }

    void Start()
    {
        _enemiesSpawned = transform.childCount;
        StartCoroutine(SpawnEnemiesCoroutine());	
    }

    void Update()
    {
        foreach (Transform trans in deadEnemies)
        {
            if (Vector3.Distance(trans.position, Player.instance.transform.position) > enemyDestroyOffset)
                Destroy(trans.gameObject);
        }
    }

    IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
			if(_introIndex < introArray.Length)
				SpawnEnemyIntro();
			else
				SpawnEnemy();
				
            yield return new WaitForSeconds(1);
        }
    }
	
	void OnBossDeath(LivingObject sender)
	{
		StartCoroutine(OnZombieBossDeath());
	}	
	
	IEnumerator OnZombieBossDeath()
	{
		yield return new WaitForSeconds(1.5f);			

		_zombieBoss = null;
	}

	void OnZombieBossLaunch(LivingObject sender)
	{
		_bossActive = false;
		StartCoroutine(OnZombieBossLaunch());
	}
	
	IEnumerator OnZombieBossLaunch()
	{
		yield return new WaitForSeconds(1.5f);		
		_bossGameObjectActive = false;
	}
	
	void SpawnEnemyIntro()
	{
		Vector3 newSpawnPos = Player.instance.transform.position + new Vector3(enemySpawnOffset, 0, 0);
        if (Vector3.Distance(newSpawnPos, _lastSpawnPosition) <= enemySpawnDistance * 2)
            return;
			
		Transform enemyTransform = (Transform)Instantiate(enemyPrefabs[introArray[_introIndex]]);
		enemyTransform.position = _lastSpawnPosition = newSpawnPos;
		enemyTransform.parent = transform;
		
		_introIndex++;
	}
	
    void SpawnEnemy()
    {
        Vector3 newSpawnPos = Player.instance.transform.position + new Vector3(enemySpawnOffset + UnityEngine.Random.Range(0, 3), 0, 0);
        if (Vector3.Distance(newSpawnPos, _lastSpawnPosition) <= enemySpawnDistance)
            return;

		if((_enemiesSpawned % bossSpawnRate) == 0)
		{
			if(_bossAlive && !_bossActive)
			{
				_zombieBoss.GetComponent<Rigidbody>().velocity = Vector3.zero;
				_zombieBoss.transform.position = _lastSpawnPosition = newSpawnPos;					
				_bossActive = true;
				_bossGameObjectActive = true;
				
				_enemiesSpawned++;
			}
			else if(!_bossAlive)
			{
				Transform enemyTransform = (Transform)Instantiate(bossPrefabs[_bossesSpawned % bossPrefabs.Length]);			
				_zombieBoss = enemyTransform.gameObject.GetComponent<ZombieBoss>();
				_zombieBoss.OnDeath += OnBossDeath;
				_zombieBoss.OnZombieBossLaunch += OnZombieBossLaunch;
				
				enemyTransform.position = _lastSpawnPosition = newSpawnPos;
				enemyTransform.parent = transform;
				_bossActive = true;	
				
				_bossesSpawned++;
				_enemiesSpawned++;
			}
		}
		else if(!_bossActive)
		{
			Transform enemyTransform = (Transform)Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)]);
			enemyTransform.position = _lastSpawnPosition = newSpawnPos;
			enemyTransform.parent = transform;
			_enemiesSpawned++;
		}
    }
}
