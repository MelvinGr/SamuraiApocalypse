using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour 
{
	public static int GlobalID { get; private set; }	
	public int id { get; private set; } 	
	
	void Awake()
	{		
		id = GlobalID++;
		foreach(Singleton singleton in (Singleton[])GameObject.FindObjectsOfType(typeof(Singleton)))
		{
			if(id > singleton.id && name == singleton.name)
				Destroy(gameObject);
		}
	}
}