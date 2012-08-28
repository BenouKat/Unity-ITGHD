using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ManageGameObject : MonoBehaviour {

	public Dictionary<double, GameObject> listarrow;

	private float time;
	
	public float zoneAppear = 10f;
// Use this for initialization
	void Awake () {
		listarrow = new Dictionary<double, GameObject>();
		time = 0f;
	}
	
	
	void FixedUpdate() {
		if(listarrow.Count > 0){
			var next = listarrow.First();
			if(next.Key < (time + zoneAppear)){
				next.Value.SetActiveRecursively(true);
				listarrow.Remove(next.Key);
			}
			
		}
		time += Time.deltaTime;
	}
	
	
	public void DoTheStartSort(){
		listarrow.OrderBy(c => c.Key);
		for(int i=0; i<listarrow.Count; i++){
			if(listarrow.ElementAt(i).Key < (time + zoneAppear)){
				listarrow.ElementAt(i).Value.SetActiveRecursively(true);
				listarrow.Remove(listarrow.ElementAt(i).Key);
				i--;
			}
		}
	
	}
	
	
	public void Add(double timetotal, GameObject go){
		if(listarrow.ContainsKey(timetotal)){
			timetotal += 0.0000001;
		}
		listarrow.Add(timetotal, go);
	}

}