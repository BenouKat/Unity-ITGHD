using UnityEngine;
using System.Collections;

public class ClickButtonEvent : MonoBehaviour {
	
	public bool isRetry;
	public InGameScript igs;
	
	void OnClick()
	{
		igs.setButtonChoice(isRetry);
	}
}
