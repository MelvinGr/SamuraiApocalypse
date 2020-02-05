using UnityEngine;
using System.Collections;

public abstract class Enemy : LivingObject
{
    public override void Start()
    {
        base.Start();

        //
    }

    public override void Update()
    {
        base.Update();

        if(GameManager.instance.isPaused)
			return;
    }

    public override void Die()
    {		
		base.Die();
		
		if(EnemyManager.instance != null)
			transform.parent = EnemyManager.instance.deadEnemies;
			
		if(Player.instance != null)
		{
			Player.instance.killCount++;
			AwardPoints();
			
			if(BloodSpatterManager.instance != null)
				BloodSpatterManager.instance.SpawnBloodSpatter(Camera.main.WorldToViewportPoint(transform.position), 1);
		}
    }
	
	public abstract void AwardPoints();
}