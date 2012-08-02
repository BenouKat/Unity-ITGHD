using UnityEngine;
using System.Collections;

public class FakeChart : MonoBehaviour {
	
	
	public GameObject ModelF;
	public GameObject ModelE;
	public GameObject ModelG;
	private float time;
	
	private bool alt;
	// Use this for initialization
	void Start () {
	
		time = 0f;
		alt = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(time >= 0.36f){
			var rand = Random.value;
			var randArrow = ((int)(Random.value*4f))*3f;
			if(rand <= 0.75f){
				var go = (GameObject) Instantiate(rand <= 0.1f ? ModelG : rand >= 0.3f ? ModelF : ModelE, ModelF.transform.position, ModelF.transform.rotation);
				go.transform.parent = gameObject.transform;
				go.transform.localPosition = new Vector3(randArrow, go.transform.localPosition.y, go.transform.localPosition.z);
				if(alt){
					go.renderer.material.color = new Color(1f, 0f, 0f, 1f);
				}else{
					go.renderer.material.color = new Color(0f, 0f, 1f, 1f);	
				}
				alt = !alt;
			}
			time = 0f;	
		}else{
			time += Time.deltaTime;
		}
	}
}
