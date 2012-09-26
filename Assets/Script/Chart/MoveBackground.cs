using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class MoveBackground : MonoBehaviour {
	
	public List<Transform> cubeBG;
	private List<Vector3> rotate;
	public float speed;
	
	
	// Use this for initialization
	void Awake () {
		var firstcubeBG = new List<GameObject>();
		firstcubeBG.AddRange(GameObject.FindGameObjectsWithTag("Background"));
		cubeBG = new List<Transform>();
		foreach(var el in firstcubeBG){
			cubeBG.Add(el.transform);	
		}
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
	void Update () {
		for(int i=0; i < cubeBG.Count;i++){
			cubeBG[i].Rotate(rotate[i]*Time.deltaTime/speed);
		}
	}
}
