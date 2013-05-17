using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	public GameObject[] positionCamera;
	private GameObject cameraGUI;
	
	public int actualPosition;
	
	public bool onMove;
	public int directionMove;
	public int stateAnim;
	
	public float speedMove;
	public float speedMoveCameraGUI;
	public Vector3 positionOutCameraGUI;
	private Vector3 basePos = new Vector3(0f, 0f, 0f);
	public float toleranceGUI;
	public float toleranceGUIAppear;
	public float toleranceBase;
	// Use this for initialization
	void Start () {
		actualPosition = 0;
		cameraGUI = transform.FindChild("CameraGUI").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(onMove)
		{
			switch(stateAnim)
			{
			case 0:
				if(Vector3.Distance(cameraGUI.transform.localPosition, -directionMove*positionOutCameraGUI) <= toleranceGUI)
				{
					cameraGUI.transform.localPosition = -directionMove*positionOutCameraGUI;	
					stateAnim++;
				}else{
					cameraGUI.transform.localPosition = Vector3.Lerp(cameraGUI.transform.localPosition, -directionMove*positionOutCameraGUI, speedMoveCameraGUI*Time.deltaTime);
				}
				break;
				
			case 1:
				if(Vector3.Distance(transform.position, positionCamera[actualPosition].transform.position) <= toleranceBase)
				{
					transform.position = positionCamera[actualPosition].transform.position;
					transform.rotation = positionCamera[actualPosition].transform.rotation;
					cameraGUI.transform.localPosition = directionMove*positionOutCameraGUI;
					stateAnim++;
				}else{
					transform.position = Vector3.Lerp(transform.position, positionCamera[actualPosition].transform.position, speedMove*Time.deltaTime);
					transform.rotation = Quaternion.Lerp(transform.rotation, positionCamera[actualPosition].transform.rotation, speedMove*Time.deltaTime);
				}
				break;
			case 2:
				if(Vector3.Distance(cameraGUI.transform.localPosition, basePos) <= toleranceGUIAppear)
				{
					cameraGUI.transform.localPosition = basePos;	
					onMove = false;
				}else{
					cameraGUI.transform.localPosition = Vector3.Lerp(cameraGUI.transform.localPosition, basePos, speedMoveCameraGUI*Time.deltaTime);
				}
				break;
			}
		}
	}
	
	
	public void moveForward()
	{
		stateAnim = 0;
		actualPosition+= 1;
		if(actualPosition >= positionCamera.Length)
		{
			actualPosition = 0;	
		}
		onMove = true;
		directionMove = 1;
	}
	
	public void moveBackward()
	{
		stateAnim = 0;
		actualPosition -= 1;
		if(actualPosition < 0)
		{
			actualPosition = positionCamera.Length - 1;	
		}
		onMove = true;
		directionMove = -1;
	}
}
