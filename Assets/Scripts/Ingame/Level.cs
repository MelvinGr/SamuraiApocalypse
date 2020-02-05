using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour
{
    public Transform segmentObjectsParent, frontObjectsParent;
    public Transform[] segmentPrefabs, frontPrefabs;

    Vector2 _segmentSize;
    int _segmentsSpawned = 0;
    float _segmentSpawnOffset;
    float _segmentDestroyOffset;
    public Vector3 _segmentSpawnOffsetV = new Vector3(0, 0, 35.65f);

    void Awake()
    {
        GameManager.level = this;

        _segmentSize = segmentPrefabs[0].GetComponent<AltMeshRenderer>().meshRenderer.bounds.size - new Vector3(1f, 0, 0);
        _segmentSpawnOffset = _segmentSize.x * 1.5f;
        _segmentDestroyOffset = _segmentSize.x * 1.5f;
    }

    void Start()
    {
        _segmentsSpawned = segmentObjectsParent.childCount;// / 2;
    }

    void Update()
    {
        foreach (Transform trans in segmentObjectsParent)
        {
            if (trans.position.x <= (Player.instance.transform.position.x - _segmentDestroyOffset))
                Destroy(trans.gameObject);
        }
		
		foreach (Transform trans in frontObjectsParent)
        {
            if (trans.position.x <= (Player.instance.transform.position.x - _segmentDestroyOffset))
                Destroy(trans.gameObject);
        }


        if ((Player.instance.transform.position.x + _segmentSpawnOffset) > (_segmentsSpawned * _segmentSize.x))
        {
            Transform segment = (Transform)Instantiate(segmentPrefabs[_segmentsSpawned % segmentPrefabs.Length]);//(UnityEngine.Object)floorSegmentPrefabs[t]);	
            segment.parent = segmentObjectsParent;		
            segment.localPosition = _segmentSpawnOffsetV + new Vector3(_segmentsSpawned * _segmentSize.x, 0, 0);

			Transform front = (Transform)Instantiate(frontPrefabs[_segmentsSpawned % frontPrefabs.Length]);	
            front.parent = frontObjectsParent;	
            front.localPosition = new Vector3(_segmentsSpawned * _segmentSize.x, 0, 0);
			
            _segmentsSpawned++;
        }
    }
}