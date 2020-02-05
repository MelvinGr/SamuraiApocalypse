using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceManager : MonoBehaviour
{
    static SliceManager _instance = null;
    public static SliceManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (SliceManager)FindObjectOfType(typeof(SliceManager));

            return _instance;
        }
    }

    public Texture2D[] sliceTextures;
    List<Slice> _sliceList = new List<Slice>();

    void OnGUI()
    {
        foreach (Slice slice in _sliceList.ToArray()) //ToArray is copy
        {
            if (slice.done)
            {
                _sliceList.Remove(slice);
                continue;
            }

            slice.Draw();
        }
    }

    public void SpawnSlice(Vector2 start, Vector2 end, float duration)
    {
		if(!enabled)
			return;
			
        if (Vector2.Distance(start, end) == 0)
            return;

        Slice slice = new Slice(sliceTextures[UnityEngine.Random.Range(0, sliceTextures.Length)], start, end, duration);
        _sliceList.Add(slice);
    }
}
