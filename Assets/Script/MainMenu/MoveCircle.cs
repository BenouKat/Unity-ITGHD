using UnityEngine;
using System.Collections;

public class MoveCircle : MonoBehaviour {

	
	public float speedRotation;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0f, speedRotation*Time.deltaTime, 0f));
	}
}
