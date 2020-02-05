using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ZombieBoss : Enemy
{
	public Transform eyeShereTransform;
	Transform _eyeObject;
	
	float _hitTimeout = 0;
	
	public LivingObjectDelegate OnZombieBossLaunch;
	
    public override void Start()
    {
        base.Start();	
		
		health = 4;
		
		_eyeObject = transform.Find("Sphere");
		if(_eyeObject != null)
			_eyeObject.parent = eyeShereTransform;

        foreach (AnimationState state in rootObject.animation)
        {
			if (!state.name.Contains("Walk"))			
			{
				state.wrapMode = WrapMode.Once;				
				state.layer = 1;
			}
			
			state.blendMode = AnimationBlendMode.Blend; 
        }
		
        rootObject.animation.Stop();
    }

    public override void Update()
    {
        base.Update();
		
		if(GameManager.instance.isPaused)
			return;

        if (health > 0)
			rootObject.animation.CrossFade("Walk");
			
		if(_hitTimeout > 0)
			_hitTimeout -= Time.deltaTime;
    }

    //If isTrigger is checked, then OnTriggerEnter is raised and objects don't collide.
    //If isTrigger is not checked, then OnCollisionEnter is raised and objects physically collide.
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
            return;

        if (collider.tag == "Weapon" && _hitTimeout <= 0)
            Hit();
	}
	
	void Hit()
	{				
		_hitTimeout = 2;
		
		Transform clone = Functions.DuplicateTranform(rootObject);
		clone.gameObject.tag = gameObject.tag;
		
		if(EnemyManager.instance != null)
			clone.transform.parent = EnemyManager.instance.deadEnemies;
		
		foreach(Transform trans in Functions.SearchHierarchyForbones(clone, "Sphere", "Particles"))
			Destroy(trans.gameObject);	
		
		health--;
		switch(health)
		{
			case 3: //helm
			{ 
				clone.name = "Helm";
		 
				// alles behalve
				Functions.RemoveChildTransform(clone, true, "Helm", "Bip01");
								
				Rigidbody rigidbody = clone.gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 0.2f; 
				rigidbody.AddForce(50f, 50f, 50f);
				rigidbody.AddTorque(Vector3.up * 10000);
				
				// allen enkel
				Functions.RemoveChildTransform(rootObject, false, "Helm");
			
				break; 
			}
			case 2: // linker arm
			{ 
				clone.name = "Left Arm";
		
				// alles behalve
				Functions.RemoveChildTransform(clone, true, "Left Arm", "Bip01");
								
				Rigidbody rigidbody = clone.gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 0.2f;
				
				// allen enkel
				Functions.RemoveChildTransform(rootObject, false, "Left Arm");
			
				break; 
			}
			case 1: // rechter arm
			{ 
				clone.name = "Right Arm";
		
				// alles behalve
				Functions.RemoveChildTransform(clone, true, "Right Arm", "Bip01");
								
				Rigidbody rigidbody = clone.gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 0.2f;
				
				// allen enkel
				Functions.RemoveChildTransform(rootObject, false, "Right Arm");
			
				break;
			}
			case 0: // doormidde
			{ 
				//if(collider.rigidbody != null)
					//Destroy(collider.rigidbody);
				if(collider != null)
					Destroy(collider);
					
				clone.name = "Split Right";
		
				// alles behalve
				Functions.RemoveChildTransform(clone, true, "Upper Body", "Right Legg", "Plate Right", "Plate Front", "Plate Back", "Bip01");
				
				//clone = rootObject;				
				rootObject.name = "Split Left";
				
				// allen enkel
				Functions.RemoveChildTransform(rootObject, false, "Upper Body", "Right Legg", "Plate Right", "Plate Front", "Plate Back");
					
				if(EnemyManager.instance != null)
					rootObject.parent = EnemyManager.instance.deadEnemies;
				else
					rootObject.parent = null;				
			
				break; 
			}
		}	
		
		if(health > 0)		
		{
			this.rigidbody.AddForce(250, 150, 0);
			rootObject.animation.Play("Death2");
		}
		else
		{
			rootObject.animation.Play("Death1");
			clone.animation.Play("Death2");
		}
		
		if(hitAudioClips.Length > 0)
			Functions.PlayAudioClip(hitAudioClips[UnityEngine.Random.Range(0, hitAudioClips.Length)], transform.position, 0.8f);
		
		if(Player.instance != null)
			Player.instance.score += 500;
		
		if(OnZombieBossLaunch != null)
			OnZombieBossLaunch(this);			
	}

	public override void AwardPoints()
	{
		Player.instance.score += 1000;	
	}
}