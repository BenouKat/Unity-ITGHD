using UnityEngine;
using System.Collections;

public class MoveCubePack : MonoBehaviour {
	
	public Vector3 speedAngleToMove;
	public Vector3 angleBorder;
	private Vector3 liveangleBorder;
	public float angleSlow;
	// Use this for initialization
	void Start () {
		liveangleBorder = new Vector3(0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(speedAngleToMove.x == 0 ? 0 : Time.deltaTime/speedAngleToMove.x, 
			speedAngleToMove.y == 0 ? 0 : Time.deltaTime/speedAngleToMove.y, 
			speedAngleToMove.z == 0 ? 0 :Time.deltaTime/speedAngleToMove.z);
		
		liveangleBorder.x += Time.deltaTime/speedAngleToMove.x;
		liveangleBorder.y += Time.deltaTime/speedAngleToMove.y;
		liveangleBorder.z += Time.deltaTime/speedAngleToMove.z;
		
		if(Mathf.Abs(liveangleBorder.x) >= angleBorder.x){
			speedAngleToMove.x = -speedAngleToMove.x;
		}
		if(Mathf.Abs(liveangleBorder.y) >= angleBorder.y){
			speedAngleToMove.y = -speedAngleToMove.y;
		}
		if(Mathf.Abs(liveangleBorder.z) >= angleBorder.z){
			speedAngleToMove.z = -speedAngleToMove.z;
		}
	}
}
