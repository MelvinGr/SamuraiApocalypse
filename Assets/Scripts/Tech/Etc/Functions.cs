using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

public class Functions
{
    public static Vector3 MultiplyVector3(Vector3 one, Vector3 two)
    {
        return new Vector3(one.x * two.x, one.y * two.y, one.z * two.z);
    }

    public static string SplitCamelCase(string input)
    {
        return Regex.Replace(Regex.Replace(input, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }

    public static Rect AddRect(Rect one, Rect two)
    {
        return new Rect(one.xMin + two.xMin, one.yMin + two.yMin, one.width + two.width, one.height + two.height);
    }

    public static GameObject CreatePlane(string name, Texture2D texture, string shader, Vector2 size)
    {
        Mesh m = new Mesh();
        m.name = name;
        m.vertices = new Vector3[] { new Vector3(-size.x, -size.y, 0.01f), new Vector3(size.x, -size.y, 0.01f), new Vector3(size.x, size.y, 0.01f), new Vector3(-size.x, size.y, 0.01f) };
        m.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();

        GameObject plane = new GameObject(name, typeof(MeshRenderer), typeof(MeshFilter));
        plane.GetComponent<MeshFilter>().mesh = m;
        plane.GetComponent<Renderer>().material.mainTexture = texture;
        plane.GetComponent<Renderer>().material.shader = Shader.Find(shader);

        return plane;
    }

    public static bool SetPrivateValue<T>(ref T target, string member, object value)
    {
        System.Reflection.FieldInfo fieldInfo = typeof(T).GetField(member, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (fieldInfo == null)
            return false;

        try
        {
            if (target is ValueType)
            {
                object b = target; // And the solution is to box the struct, keep a reference to the boxed instance, and unbox it when you're done. (struct is ValueType)
                fieldInfo.SetValue(b, value);
                target = (T)b;
            }
            else
                fieldInfo.SetValue(target, value);

            return true;
        }
        catch 
        {
            return false;
        }
    }

    public static int LayerMasks(params String[] layerNames)
    {
        int value = 0;
        foreach (String layername in layerNames)
        {
            bool inverted = (layername[0] == '!');
            if (inverted)
                value |= ~(1 << LayerMask.NameToLayer(layername.Replace("!", ""))); // klopt niet?
            else
                value |= (1 << LayerMask.NameToLayer(layername));
        }

        return value;
    }

    public static Transform[] SearchHierarchyForbones(Transform current, params string[] names)
    {
		List<Transform> bones = new List<Transform>();
		foreach(String name in names)
		{
			if (current.name.Contains(name))
				bones.Add(current);

			for (int i = 0; i < current.GetChildCount(); ++i)
			{
				bones.AddRange(SearchHierarchyForbones(current.GetChild(i), name));
			}
		}
		
        return bones.ToArray();
    }

    public static Transform[] RemoveBoneStructure(Transform rootTransform, params String[] childsToRemove)
    {
        List<Transform> removedTransforms = new List<Transform>();
        foreach (Transform transform in rootTransform)
        {			
			foreach(string name in childsToRemove)
			{
				if(transform.name.Contains(name))
				{
					SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
					if (skinnedMeshRenderer == null)
						continue;

					MeshFilter meshFilter = transform.gameObject.GetComponent<MeshFilter>();
					if(meshFilter == null)
						meshFilter = transform.gameObject.AddComponent<MeshFilter>();
					
					meshFilter.mesh = skinnedMeshRenderer.sharedMesh;

					MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
					if(meshRenderer == null)
						meshRenderer = transform.gameObject.AddComponent<MeshRenderer>();
					
					meshRenderer.materials[0] = skinnedMeshRenderer.sharedMaterial;
					meshRenderer.materials[0].mainTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
					meshRenderer.materials[0].shader = skinnedMeshRenderer.sharedMaterial.shader;

					UnityEngine.Object.Destroy(skinnedMeshRenderer);
					removedTransforms.Add(transform);
					
					break;
				}
			}
        }

        return removedTransforms.ToArray();
    }
	
	public static GameObject CombineMeshes(MeshFilter[] meshFilters, bool destroy)
	{
		List<CombineInstance> combines = new List<CombineInstance>();
		foreach(MeshFilter meshFilter in meshFilters)
		{
			combines.Add
			(
				new CombineInstance
				{
					mesh = meshFilter.sharedMesh,
					transform = meshFilter.transform.localToWorldMatrix
				}
			);
			
			if(destroy)
				UnityEngine.GameObject.Destroy(meshFilter.gameObject);
		}
		
		GameObject newGameObject = new GameObject("");
		
		MeshFilter meshFilte = newGameObject.AddComponent<MeshFilter>();
		meshFilte.mesh = new Mesh();
		meshFilte.mesh.CombineMeshes(combines.ToArray());
		
		newGameObject.AddComponent<MeshRenderer>();
		return newGameObject;
	}
	
	public static void CloneGameObjectMeshRenderer(GameObject old, GameObject neww)
	{
		MeshRenderer oldMeshRenderer = old.GetComponent<MeshRenderer>();
		MeshRenderer newMeshRenderer = neww.GetComponent<MeshRenderer>();
		newMeshRenderer.materials[0] = oldMeshRenderer.sharedMaterial;
		newMeshRenderer.materials[0].mainTexture = oldMeshRenderer.sharedMaterial.mainTexture;
		newMeshRenderer.materials[0].shader = oldMeshRenderer.sharedMaterial.shader;
	}

    public static int RemoveChildTransform(Transform transform, bool allBut, params string[] childNames)
    {
        int removedCount = 0;
        foreach (Transform trans in transform)
        {
			if(!allBut)
			{
				foreach(string name in childNames)
				{					
					//if (((IList<string>)childNames).Contains(trans.name)) // cast omdat .net < 3				
					if(trans.name.Contains(name))
					{
						UnityEngine.Object.Destroy(trans.gameObject);
						removedCount++;
						
						break;
					}
				}
			}
			else
			{
				if(!childNames.Contains(trans.name))
				{
					UnityEngine.Object.Destroy(trans.gameObject);
					removedCount++;
				}
			}
        }

        return removedCount;
    }

    public static Transform DuplicateTranform(Transform transform)
    {
        Transform rootObjectCopy = (Transform)GameObject.Instantiate(transform);
        rootObjectCopy.position = transform.position;
        rootObjectCopy.rotation = transform.rotation;
        return rootObjectCopy;
    }
	
	public static T[] GetComponentsInChildren<T>(Transform t)
	{
		List<T> renderers = new List<T>();

		if(t.GetComponent(typeof(T)) != null)
			renderers.Add((T)System.Convert.ChangeType(t.GetComponent(typeof(T)), typeof(T)));

		foreach(Transform child in t)
			renderers.AddRange(GetComponentsInChildren<T>(child));
			
		return renderers.ToArray();
	}
	
	public static T InstantiateComponent<T>(GameObject original)
	{
		object obj = ((GameObject)GameObject.Instantiate(original)).GetComponent(typeof(T));
		return (T)System.Convert.ChangeType(obj, typeof(T));
	}
	
	public static Transform[] GetChildTransformsAsArray(Transform transform)
    {
        List<Transform> childs = new List<Transform>();
        foreach (Transform trans in transform)
			childs.Add(trans);

        return childs.ToArray();
    }
	
	public static AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume = 1f, int priority = 128)
	{
        GameObject go = new GameObject(clip.name);
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
		source.rolloffMode = AudioRolloffMode.Linear;
        source.clip = clip;
        source.volume = volume;
		source.priority = priority;
        source.Play();
        UnityEngine.Object.Destroy(go, clip.length);
        return source;
    }
}