using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverGUI : MonoBehaviour  
{
	public GUIText scoreCount;
	public GUITexture newHighScore;
	public Texture2D[] fallingGUITextures;
	
	public int playerScoreIncrement = 100, playerHighScoreIncrement = 100;	
	public Vector2 fallStartPosition = new Vector2(355, -5);
	
	public float fallSpawnTime = 0.2f;
	public float fallSpeed = 0.1f;
	public float lowestHeight = -2f;
	
	int _fallCounter, _fallCount = 2;	
	int _playerScore, _playerHighScore; 
	List<Transform> _fallingTransforms = new List<Transform>();
	GameObject _fallingGameObjects;
	
	void Awake()
	{
		newHighScore.active = false;
		_fallingGameObjects = new GameObject("FallingGameObjects");
		_fallingGameObjects.transform.position = Vector3.zero;
		
		//GameManager.instance.playerScore = 5000; 
	}
	
	void Start() 
	{
		fallStartPosition = Camera.main.ScreenToViewportPoint(fallStartPosition);
		_fallCount = (GameManager.instance.playerScore / 100);
		
		StartCoroutine(CounterCoRoutine());
		StartCoroutine(SpawnFallingCoRoutine());
	}
	
	void Update()
	{
		if(InputManager.instance.touchCount > 0)
			_playerScore = GameManager.instance.playerScore;
			
		foreach(Transform trans in _fallingTransforms)
		{
			if(!trans.gameObject.active)
				continue;
					
			trans.position -= new Vector3(0, fallSpeed / 30f, 0);
			if(trans.position.y < lowestHeight)
			{
				Destroy(trans.guiTexture);
				trans.gameObject.active = false;
			}
		}
	}

	IEnumerator CounterCoRoutine()
	{
		while(_playerScore <= GameManager.instance.playerScore)
		{
			_playerScore += playerScoreIncrement;
			scoreCount.text = System.String.Format("{0:00000}", _playerScore);
			
			yield return new WaitForSeconds(1f / 30f);			
		}
		
		scoreCount.text = System.String.Format("{0:00000}", GameManager.instance.playerScore);
	}
	
	IEnumerator SpawnFallingCoRoutine()
	{
		while(_playerScore <= GameManager.instance.playerScore)//_fallCounter <= _fallCount)
		{
			Texture2D zombieTexture = fallingGUITextures[Random.Range(0, fallingGUITextures.Length)];
			
			GameObject zombie = new GameObject("", typeof(GUITexture));
			zombie.transform.position = new Vector3(fallStartPosition.x, 1 - fallStartPosition.y, 2);
			zombie.transform.localScale = new Vector3(0, 0, 1);
			zombie.transform.parent = _fallingGameObjects.transform;
			zombie.guiTexture.texture = zombieTexture;
			zombie.guiTexture.pixelInset = new Rect(-zombieTexture.width / 2, 0, zombieTexture.width, zombieTexture.height);
				
			_fallingTransforms.Add(zombie.transform);
			_fallCounter++;
			
			yield return new WaitForSeconds(fallSpawnTime);			
		}
		
		newHighScore.active = (GameManager.instance.playerScore > GameManager.instance.playerHighScore);
	}
}
