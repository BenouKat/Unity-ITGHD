using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	public GameObject[] positionCamera;
	private GameObject cameraGUI;
	
	private int actualPosition;
	
	private bool onMove;
	private int stateAnim;
	
	public float speedMove;
	public float speedMoveCameraGUI;
	public Transform positionOutCameraGUI;
	private Vector3 basePos = new Vector3(0f, 0f, 0f);
	public float toleranceGUI;
	public float toleranceGUIAppear;
	public float toleranceBase;
	// Use this for initialization
	void Start () {
		actualPosition = 0;
		cameraGUI = transform.FindChild("GUICamera").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(onMove)
		{
			switch(stateAnim)
			{
			case 0:
				if(Quaternion.Angle(cameraGUI.transform.localRotation, positionOutCameraGUI.localRotation) <= toleranceGUI)
				{
					cameraGUI.transform.localRotation = positionOutCameraGUI.localRotation;	
					stateAnim++;
				}else{
					cameraGUI.transform.localRotation = Quaternion.Lerp(cameraGUI.transform.localRotation, positionOutCameraGUI.localRotation, speedMoveCameraGUI*Time.deltaTime);
				}
				break;
				
			case 1:
				if(Vector3.Distance(transform.position, positionCamera[actualPosition].transform.position) <= toleranceBase)
				{
					transform.position = positionCamera[actualPosition].transform.position;
					transform.rotation = positionCamera[actualPosition].transform.rotation;
					stateAnim++;
				}else{
					transform.position = Vector3.Lerp(transform.position, positionCamera[actualPosition].transform.position, speedMove*Time.deltaTime);
					transform.rotation = Quaternion.Lerp(transform.rotation, positionCamera[actualPosition].transform.rotation, speedMove*Time.deltaTime);
				}
				break;
			case 2:
				if(Quaternion.Angle(cameraGUI.transform.localRotation, Quaternion.Euler(basePos)) <= toleranceGUIAppear)
				{
					cameraGUI.transform.localRotation = Quaternion.Euler(basePos);	
					onMove = false;
				}else{
					cameraGUI.transform.localRotation = Quaternion.Lerp(cameraGUI.transform.localRotation, Quaternion.Euler(basePos), speedMoveCameraGUI*Time.deltaTime);
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
	}
}
