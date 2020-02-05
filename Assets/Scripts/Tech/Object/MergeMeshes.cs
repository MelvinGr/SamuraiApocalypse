using UnityEngine;
using System.Collections;

public class MergeMeshes : MonoBehaviour 
{
	void Start() 
	{
		GameObject rightObject = Functions.CombineMeshes(Functions.GetComponentsInChildren<MeshFilter>(transform), false);
		Functions.CloneGameObjectMeshRenderer(gameObject.GetComponent<AltMeshRenderer>().meshRenderer.gameObject, rightObject);
	}
}
