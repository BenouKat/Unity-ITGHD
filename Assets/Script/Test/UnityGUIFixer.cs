using UnityEngine;
using System.Collections;

public class UnityGUIFixer : MonoBehaviour {
	
	Rect bounds = new Rect(-2000,-2000,0,0);
	
	void Awake()
	{
		DontDestroyOnLoad(this);	
	}
	
	void OnGUI()
    {
        string controlName = gameObject.GetHashCode().ToString();

        GUI.SetNextControlName(controlName);

        GUI.TextField (bounds, "", 0);
    }
}
