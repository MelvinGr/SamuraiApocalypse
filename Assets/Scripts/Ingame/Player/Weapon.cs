using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public enum WeaponSide { Left, Right };

    public Transform rootObject;

    new public string name;
    public WeaponSide weaponSide;
	
	public Transform trailRendererParent;
	public TrailRenderer trailRenderer;
	
	public void InstantiateTrailRenderer(float time)
	{
		if(trailRenderer == null)
			return;
			
		TrailRenderer tmp = Functions.InstantiateComponent<TrailRenderer>(trailRenderer.gameObject);
		tmp.transform.parent = trailRendererParent;
		tmp.transform.localPosition = Vector3.zero;
		tmp.gameObject.active = true;
		tmp.time = 0.2f;
		
		tmp.gameObject.AddComponent<DeAttachAfterTime>().StartTimer(time / 3);
	}
}
