using UnityEngine;
using System.Collections;

public class ArrowFake : MonoBehaviour {
	
	public ParticleSystem ParticleToPlay;
	public ParticleSystem Halo;
	
	public float speed;
	
	private bool touched;
	// Use this for initialization
	void Start () {
		touched = false;	
	}
	
	// Update is called once per frame
	void Update () {
		if(!touched){
			if(transform.localPosition.y >= 0f){
				renderer.enabled = false;
				ParticleToPlay.Play();
				if(Halo != null) Halo.Play();
				Destroy(gameObject, 1f);
				touched = true;
			}else{
				transform.localPosition += new Vector3(0f, Time.deltaTime/speed, 0f);
			}
			
		}
		
	}
}
