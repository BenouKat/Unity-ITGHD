using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ManageGameObject : MonoBehaviour {

	public Dictionary<float, GameObject> listarrow;
	
	public Transform cameraTransform;
	
	public float zoneAppear = 10f;
// Use this for initialization
	void Awake () {
		listarrow = new Dictionary<float, GameObject>();
	}
	
	
	void FixedUpdate() {
		if(listarrow.Any()){
			var next = listarrow.First();
			if(next.Key > (cameraTransform.position.y - zoneAppear)){
				next.Value.SetActiveRecursively(true);
				listarrow.Remove(next.Key);
			}
			
		}
	}
	
	
	public void DoTheStartSort(){
		listarrow.OrderByDescending(c => c.Key);
		for(int i=0; i<listarrow.Count; i++){
			if(listarrow.ElementAt(i).Key > (cameraTransform.position.y - zoneAppear)){
				listarrow.ElementAt(i).Value.SetActiveRecursively(true);
				listarrow.Remove(listarrow.ElementAt(i).Key);
				i--;
			}
		}
	
	}
	
	
	public void Add(GameObject go){
		var ypos = go.transform.position.y;
		var counter = 0;
		while(listarrow.ContainsKey(ypos) && counter < 10){
			ypos -= 0.01f;
			counter++;
		}
		if(!listarrow.ContainsKey(ypos)) listarrow.Add(ypos, go);
	}

}