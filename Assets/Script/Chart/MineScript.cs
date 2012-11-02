using UnityEngine;
using System.Collections;



public class MineScript : MonoBehaviour {
	public GameObject arrowLeft;
	public GameObject Engine;
	private Arrow associatedArrow;
	
	public string state;
	
	private InGameScript igs;
	private KeyCode associatedKeyCode1;
	private KeyCode associatedKeyCode2;
	public bool missed;
	public bool valid;
	public bool alreadyScored;
	
	public AudioSource secondAudioSource;
	public AudioClip explosionSound;
	
	private delegate void sMethod();
	private sMethod pm;
	
	// Use this for initialization
	void Start () {
		igs = Engine.GetComponent<InGameScript>();

		switch((int)transform.position.x){
			case 0:
				state = "left";
				associatedKeyCode1 = DataManager.Instance.KeyCodeLeft;
				associatedKeyCode2 = DataManager.Instance.SecondaryKeyCodeLeft;
				this.pm = igs.StartParticleMineLeft;
				break;
			case 2:
				state = "down";
				associatedKeyCode1 = DataManager.Instance.KeyCodeDown;
				associatedKeyCode2 = DataManager.Instance.SecondaryKeyCodeDown;
				this.pm = igs.StartParticleMineDown;
				break;
			case 4:
				state = "up";
				associatedKeyCode1 = DataManager.Instance.KeyCodeUp;
				associatedKeyCode2 = DataManager.Instance.SecondaryKeyCodeUp;
				this.pm = igs.StartParticleMineUp;
				break;
			case 6:
				state = "right";
				associatedKeyCode1 = DataManager.Instance.KeyCodeRight;
				associatedKeyCode2 = DataManager.Instance.SecondaryKeyCodeRight;
				this.pm = igs.StartParticleMineRight;
				break;
		}
		missed = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!missed){
			if(associatedArrow.time <= igs.getTotalTimeChart() && (Input.GetKey(associatedKeyCode1) || Input.GetKey(associatedKeyCode2))){
				this.pm();
				secondAudioSource.PlayOneShot(explosionSound);
				igs.GainScoreAndLife("MINE");
				igs.AddMineToScoreCombo();
				igs.removeMineFromList(associatedArrow, state);
				DestroyImmediate(arrowLeft);
			}else  if(associatedArrow.time <= igs.getTotalTimeChart()){
				missed = true;
				igs.removeMineFromList(associatedArrow, state);
			}
		}
		
	}
	
	public void setArrowAssociated(Arrow ar){
		associatedArrow = ar;
	}
}
