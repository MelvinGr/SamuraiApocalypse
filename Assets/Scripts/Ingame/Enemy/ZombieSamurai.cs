using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ZombieSamurai : Enemy
{
	public bool isJumper;
	public float jumperJumpDistance = 7f;
	bool _isJumping, _didJump;
	
    public override void Start()
    {
        base.Start();

		foreach (AnimationState state in rootObject.GetComponent<Animation>())
        {
			if (!state.name.Contains("Walk"))			
			{
				state.wrapMode = WrapMode.Once;
				state.layer = 1;
			}
			
			state.blendMode = AnimationBlendMode.Blend; 
			
			if (state.name.Contains("Jump"))			
				state.speed = 1.5f;
        }
		
        rootObject.GetComponent<Animation>().Stop();
    }

    public override void Update()
    {
        base.Update();
		
		if(GameManager.instance.isPaused)
		{
			rootObject.GetComponent<Animation>().enabled = false;
			return;
		}
		else
			rootObject.GetComponent<Animation>().enabled = true;
		
        if (health > 0)
        {
            if(isJumper)
			{
				if(!_didJump)
				{					
					if(_isJumping)
					{
						rootObject.GetComponent<Animation>().Play("Jump");
						_didJump = true;
					}
					else
					{
						_isJumping = (Vector3.Distance(transform.position, Player.instance.transform.position) < jumperJumpDistance);
						rootObject.GetComponent<Animation>().CrossFade("Walk");
					}
				}
				else
				{
					rootObject.GetComponent<Animation>().CrossFade("Walk");
					transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
				}
			}
			else
			{
				rootObject.GetComponent<Animation>().CrossFade("Walk");
				transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
			}
        }
    }

    //If isTrigger is checked, then OnTriggerEnter is raised and objects don't collide.
    //If isTrigger is not checked, then OnCollisionEnter is raised and objects physically collide.
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
            return;

        if (collider.tag == "Weapon")
            health--;

        //
    }

    public override void Die()
    {
        base.Die();
		
		if(GetComponent<Collider>().GetComponent<Rigidbody>() != null)
			Destroy(GetComponent<Collider>().GetComponent<Rigidbody>());
        if(GetComponent<Collider>() != null)
			Destroy(GetComponent<Collider>());
		
		rootObject.GetComponent<Animation>().Stop("Walk");

		switch(Player.instance.currentAnimation)
		{
			case Player.PlayerAnimation.HorizontalSlash:
			{
				rootObject.GetComponent<Animation>().Play("Death01");				

				foreach (Transform trans in Functions.RemoveBoneStructure(rootObject, "Samurai_Zombie_Hoofd"))
				{
					trans.parent = transform;

					Rigidbody rigidbody = trans.gameObject.AddComponent<Rigidbody>();
					rigidbody.mass = 0.2f;
					rigidbody.AddForce(50f, 50f, 50f);
					rigidbody.AddTorque(Vector3.up * 10000);
				}
				
				break;
			}
			
			case Player.PlayerAnimation.VerticalSlash:
			case Player.PlayerAnimation.VerticalSlashTopDown:
			case Player.PlayerAnimation.DiagonalSlash:
			{				
				Transform leftSide = Functions.DuplicateTranform(rootObject);
				leftSide.transform.parent = transform;
				leftSide.gameObject.name = leftSide.gameObject.name.Replace("(Clone)", "(Left Side)");
				Functions.RemoveChildTransform(leftSide, false, "Samurai_Zombie_Body_01", "Samurai_Zombie_Body_02", 
					"Samurai_Zombie_Hoofd", "Samurai_Zombie_Linker_Schouderflapje");

				AnimationPlayer animationPlayer = leftSide.gameObject.AddComponent<AnimationPlayer>();
				animationPlayer.animationName = "Death02";	
				animationPlayer.playSingle = true;
				
				Rigidbody rigidbody = leftSide.gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 0.2f;
				rigidbody.useGravity = false;
				if(Player.instance.currentAnimation == Player.PlayerAnimation.VerticalSlashTopDown)
				{
					rigidbody.AddForce(100f, 0f, 0);
					rigidbody.AddTorque(Vector3.right * 10000);
				}
				else
				{		
					rigidbody.AddForce(0f, 100f, 0f);
					rigidbody.AddTorque(Vector3.up * 10000);
				}
				
				///////////////////
					
				Transform rightSide = rootObject;	
				rightSide.gameObject.name += "(Right Side)";			
				Functions.RemoveChildTransform(rightSide, false, "Samurai_Zombie_Body_03", "Samurai_Zombie_Body_04", "flapje", "lapje"); 
				
				animationPlayer = rightSide.gameObject.AddComponent<AnimationPlayer>();
				animationPlayer.animationName = "Death02";	
				animationPlayer.playSingle = true;
				
				Destroy(this);
				
				break;
			}
			
			default:
			{
				//Debug.Log("Unknown PlayerAnimation: " + Player.instance.currentAnimation);
				break;
			}
		}	
    }
	
	public override void AwardPoints()
	{
		Player.instance.score += 100 + (isJumper ? 100 : 0);
	}
}