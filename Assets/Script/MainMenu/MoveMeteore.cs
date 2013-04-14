using UnityEngine;
using System.Collections;

public class MoveMeteore : MonoBehaviour {
	
	public float speedMeteore;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0f, 0f, -speedMeteore*Time.deltaTime);
	}
}
