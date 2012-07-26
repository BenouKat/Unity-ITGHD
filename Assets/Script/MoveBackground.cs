using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class MoveBackground : MonoBehaviour {
	
	private List<GameObject> cubeBG;
	private List<Vector3> rotate;
	public float speed;
	// Use this for initialization
	void Start () {
		cubeBG = new List<GameObject>();
		cubeBG.AddRange(GameObject.FindGameObjectsWithTag("Background"));
		
		rotate = new List<Vector3>();
		for(int i = 0; i < cubeBG.Count; i++){
			var x = 1 - Random.value*2;
			var y = 1 - Random.value*2;
			var z = 1 - Random.value*2;
			//Debug.Log(x + " " + y + " " + z); 
			rotate.Add(new Vector3(x, y, z));
		}
		
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		for(int i=0; i < cubeBG.Count;i++){
			cubeBG[i].transform.Rotate(rotate[i]*Time.deltaTime/speed);
		}
	}
}
