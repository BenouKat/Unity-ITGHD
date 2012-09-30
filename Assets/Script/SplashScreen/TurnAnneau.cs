using UnityEngine;
using System.Collections;

public class TurnAnneau : MonoBehaviour {
	
	public float speedRotate;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0f, 0f, Time.deltaTime/speedRotate);
	}
}
