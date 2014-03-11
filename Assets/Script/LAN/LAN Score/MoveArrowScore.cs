using UnityEngine;
using System.Collections;

public class MoveArrowScore : MonoBehaviour {
	
	
	public float speedArrows;
	
	public float targetExplode;
	
	private bool isExploded;
	// Use this for initialization
	void Start () {
		isExploded = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isExploded)
		{
			transform.localPosition += new Vector3(0f, 0f, speedArrows*Time.deltaTime);
			if(transform.position.z > targetExplode)
			{
				isExploded = true;
				renderer.enabled = false;
				transform.FindChild("Destroy Cube Particle").gameObject.SetActive(true);
				transform.FindChild("Explode Cube Particle").gameObject.SetActive(true);
				Destroy(gameObject, 3f);
			}
		}
	}
}
