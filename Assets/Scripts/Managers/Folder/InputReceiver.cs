using UnityEngine;
using System.Collections;

public abstract class InputReceiver : MonoBehaviour
{
	internal string _pressedName;
	public string cleanPressedName
	{
		get { return (_pressedName != null ? _pressedName.Substring(0, _pressedName.IndexOf('_')) : ""); }
	}
	
	[HideInInspector]
	public GUIElement[] guiElements;
	
	public virtual void Start()
	{
		guiElements = gameObject.GetComponentsInChildren<GUIElement>();
	}
	
    public virtual void OnTouch(GUIElement guiElement)
    {
		_pressedName = guiElement.name;
	}
}