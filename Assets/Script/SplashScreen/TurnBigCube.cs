using UnityEngine;
using System.Collections;

public class TurnBigCube : MonoBehaviour {
	
	public Vector3 speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Time.deltaTime/speed.x, Time.deltaTime/speed.y, Time.deltaTime/speed.z);
	}
}
