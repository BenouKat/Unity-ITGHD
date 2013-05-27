using UnityEngine;
using System.Collections;

public class OnButtonClicked : MonoBehaviour {

	public int buttonID;
	
	public CameraMove cm;
	
	void OnClick()
	{
		switch(buttonID)
		{
		case 0:
			cm.moveForward();
			break;
		case 1:
			cm.moveBackward();
			break;
		default:
			break;
		}
	}
}
