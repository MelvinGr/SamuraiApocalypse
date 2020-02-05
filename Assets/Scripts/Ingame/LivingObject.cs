using UnityEngine;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class LivingObject : MonoBehaviour
{
    public delegate void LivingObjectDelegate(LivingObject sender);
    public LivingObjectDelegate OnDeath;
	
	[System.Serializable]
	public class Particle
	{
		public Transform parent;
		public ParticleSystem particleSystem;
		public int healthActivate;
		public float startRotation;
		public Vector3 transfomRotation;
		public bool continuous;
		[HideInInspector]
		public bool didPlay;
	}

    public int _health; // public voor de inspector
    public int health
    {
        get { return _health; }
        set
        {
            _health = value;
			
			foreach(Particle particle in particleSystems)
			{
				if(particle.healthActivate == _health)
				{
					if(particle.parent == null || particle.particleSystem == null)
						continue;
					
					particle.particleSystem.transform.eulerAngles = particle.transfomRotation;					
					particle.particleSystem.startRotation = particle.startRotation;
					
					if(!particle.didPlay)
					{
						particle.particleSystem.loop = true;
						particle.particleSystem.Play();
						particle.didPlay = true;
					}
				}	
			}
			
            if (_health == 0)
            {
                Die();

                if (OnDeath != null)
                    OnDeath(this);
            }
        }
    }

    public Transform rootObject;
	
	public float moveSpeed = 2;

	public Particle[] particleSystems;
	
	public AudioClip[] hitAudioClips;
	public AudioClip[] dieAudioClips;

    public virtual void Awake()
    {
       foreach(Particle particle in particleSystems)
		{
			if(particle.parent == null || particle.particleSystem == null)
				continue;
				
			particle.particleSystem = Functions.InstantiateComponent<ParticleSystem>(particle.particleSystem.gameObject);			
			particle.particleSystem.transform.parent = particle.parent;
			particle.particleSystem.transform.localPosition = Vector3.zero;
			particle.particleSystem.transform.localRotation = Quaternion.identity;
			particle.particleSystem.transform.localScale = Vector3.one;
		}
    }

    public virtual void Start()
    {
        //
    }

    public virtual void Update()
    {
		if(GameManager.instance.isPaused)
			return;
			
        //
    }

    public virtual void Die()
	{
		if(dieAudioClips.Length > 0)
			Functions.PlayAudioClip(dieAudioClips[UnityEngine.Random.Range(0, dieAudioClips.Length)], transform.position, 0.8f);
	}
}