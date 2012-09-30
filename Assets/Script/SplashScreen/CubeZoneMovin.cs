using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CubeZoneMovin : MonoBehaviour {

	private List<Vector3> rotate;
	private List<Transform> cubeBG;
	public float speed;
	
	
	// Use this for initialization
	void Awake () {
		cubeBG = new List<Transform>();
		for(int i=0;i<transform.childCount;i++){
			var x = 180 - Random.value*360;
			var y = 180 - Random.value*360;
			var z = 180 - Random.value*360;
			transform.GetChild(i).transform.Rotate(x, y, z);
			cubeBG.Add(transform.GetChild(i).transform);	
		}
		rotate = new List<Vector3>();
		for(int i = 0; i < cubeBG.Count; i++){
			var x = 1 - Random.value*2;
			var y = 1 - Random.value*2;
			var z = 1 - Random.value*2;
			//Debug.Log(x + " " + y + " " + z); 
			rotate.Add(new Vector3(x, y, z));
		}
		speed -= (Random.value/15f);
		
	}
	
	// Update is called once per frame
	void Update () {
		for(int i=0; i < cubeBG.Count;i++){
			cubeBG[i].Rotate(rotate[i]*Time.deltaTime/(speed));
		}
	}
}
