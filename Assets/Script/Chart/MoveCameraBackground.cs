using UnityEngine;
using System.Collections;

public class MoveCameraBackground : MonoBehaviour {
	
	public float speed;
	public float timeBeforeChange;
	private float timeT;
	private Vector3 rotationPoint;
	private Transform me;
	
	private float x;
	private float y;
	private float z;
	private float xdiff;
	private float ydiff;
	private float zdiff;
	
	private Vector3 poolVector = new Vector3(0f, 0f, 0f);
	// Use this for initialization
	void Start () {
		timeT = timeBeforeChange;
		rotationPoint = new Vector3( 0f, 0f, 1f);
		me = gameObject.transform;
		x = 0f;
		y = 0f;
		z = 0f;
		xdiff = 0f;
		ydiff = 0f;
		zdiff = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(timeBeforeChange <= timeT){
			timeT = 0f;
			x = Random.value*2f - 1f;
			y = Random.value - 0.5f > 0 ? Random.value*0.7f : -Random.value*0.1f;
			z = Random.value*2f - 1f;
			xdiff = Mathf.Abs(x - rotationPoint.x);
			ydiff = Mathf.Abs(y - rotationPoint.y);
			zdiff = Mathf.Abs(z - rotationPoint.z);
			poolVector.x = xdiff <= 1f ? x : x - (xdiff - 1f);
			poolVector.y = ydiff <= 1f ? y : y - (ydiff - 1f);
			poolVector.z = zdiff <= 1f ? z : z - (zdiff - 1f);
			rotationPoint = poolVector;
			
		}
		timeT += Time.deltaTime;
		me.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationPoint), Time.deltaTime/(speed));
	}
}
