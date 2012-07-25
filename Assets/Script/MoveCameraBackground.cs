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
	}
	
	// Update is called once per frame
	void Update () {
		
		if(timeBeforeChange <= timeT){
			timeT = 0f;
			var y = Random.value - 0.5f;
			rotationPoint = new Vector3( Random.value*2f - 1f, y > 0f ? Random.value*0.7f : -Random.value*0.1f, Random.value*2f - 1f);
		}
		timeT += Time.deltaTime;
		gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotationPoint), Time.deltaTime/(speed));
	}
}
