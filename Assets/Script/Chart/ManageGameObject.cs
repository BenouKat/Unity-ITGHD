using UnityEngine;
using System.Collections;
using System.Generic;
using System.Linq;

public class ManageGameObject : MonoBehaviour {

	public Dictionary<double, GameObject> listarrow;

	private float time;
	
	public float zoneAppear = 10f;
// Use this for initialization
	void Start () {
		
		time = 0f;
	}
	
	
	void FixedUpdate() {
		if(listarrow.Count > 0){
			var next = listarrow.First();
			if(next.Key < (time + zoneAppear)){
				next.Value.SetActiveRecursivly(true);
				listarrow.Remove(next.Key);
			}
			
		}
		
	}
	
	
	void DoTheStartSort(){
		listarrow.OrderBy(c => c.Key);
		for(int i=0; i<listarrow.Count; i++){
			if(listarrow.ElementAt(i).Key < (time + zoneAppear)){
				listarrow.ElementAt(i).Value.SetActiveRecursivly(true);
				listarrow.Remove(listarrow.ElementAt(i).Key);
				i--;
			}
		}
	
	}

}