using UnityEngine;
using System.Collections;

public class MoveCameraBackground : MonoBehaviour {
	
	public float speed;
	public float timeBeforeChange;
	private float timeT;
	private Vector3 rotationPoint;
	// Use this for initialization
	void Start () {
		timeT = timeBeforeChange;
		rotationPoint = new Vector3( 0f, 0f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(timeBeforeChange <= timeT){
			timeT = 0f;
			var x = Random.value*2f - 1f;
			var y = Random.value - 0.5f > 0 ? Random.value*0.7f : -Random.value*0.1f;
			var z = Random.value*2f - 1f;
			var xdiff = Mathf.Abs(x - rotationPoint.x);
			var ydiff = Mathf.Abs(y - rotationPoint.y);
			var zdiff = Mathf.Abs(z - rotationPoint.z);
			var truex = xdiff <= 1f ? x : x - (xdiff - 1f);
			var truey = ydiff <= 1f ? y : y - (ydiff - 1f);
			var truez = zdiff <= 1f ? z : z - (zdiff - 1f);
			rotationPoint = new Vector3(truex , truey , truez);
			
		}
		timeT += Time.deltaTime;
		gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationPoint), Time.deltaTime/(speed));
	}
}
