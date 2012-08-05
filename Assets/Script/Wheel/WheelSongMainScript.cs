using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class WheelSongMainScript : MonoBehaviour {
	
	
	public GameObject miniCubePack;
	public GUISkin skin;
	
	private List<string> packs;
	private int numberPack;
	private int numberCube;
	private Dictionary<GameObject, float> allCubes;
	
	public float x10;
	public float xm10;
	public float y;
	public float wd;
	public float ht;
	
	public Rect posBackward;
	public Rect posForward;
	public float ecart;
	
	
	public bool movinForward;
	public bool movinBackward;
	public float speedMove;
	public float limite;
	
	public float decalFade;
	public float decalFadeM;
	// Use this for initialization
	void Start () {
		numberPack = 0;
		numberCube = 3;
		packs = new List<string>();
		LoadManager.Instance.Loading();
		foreach(var el in LoadManager.Instance.ListSong().Keys){
			packs.Add(el);	
		}
		createPacks();
		decalFade = 0f;
		decalFadeM = 0f;
	}
	
	// Update is called once per frame
	public void OnGUI () {
		GUI.skin = skin;
		var decal = ((Screen.width - wd*Screen.width)/2);
		GUI.Label(new Rect((xm10*3f + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 3)));
		GUI.Label(new Rect((xm10*2f + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 2)));
		GUI.Label(new Rect((xm10 + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 1)));
		GUI.Label(new Rect(decalFade > 0 ? decalFade*Screen.width + decal : decalFadeM*Screen.width + decal  , y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(numberPack));
		GUI.Label(new Rect((x10 + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 1)));
		GUI.Label(new Rect((x10*2f + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 2)));
		GUI.Label(new Rect((x10*3f + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 3)));
		if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","LBackward") && !movinBackward){
			allCubes.ElementAt(numberCube).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
			numberCube = PrevIntCube(numberCube, 1);
			allCubes.ElementAt(numberCube).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
			movinBackward = true;
		}
		GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height + ecart*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","Backward");
		
		if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","LForward") && !movinForward)
		{
			allCubes.ElementAt(numberCube).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
			numberCube = NextIntCube(numberCube, 1);
			allCubes.ElementAt(numberCube).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
			movinForward = true;
		}
		GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height + ecart*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","Forward");
	}
	
	void Update(){
		if(movinForward){
			foreach(var el in allCubes){
				el.Key.transform.position = Vector3.Lerp(el.Key.transform.position, new Vector3(el.Value - 10f, el.Key.transform.position.y, el.Key.transform.position.z), Time.deltaTime/speedMove);
			}
			decalFade = Mathf.Lerp(decalFade, -x10, Time.deltaTime/speedMove);
			decalFadeM = Mathf.Lerp(decalFadeM, xm10, Time.deltaTime/speedMove);
			if(Mathf.Abs(allCubes.First().Key.transform.position.x - (allCubes.First().Value - 10f)) <= limite){
				movinForward = false;
				decalFade = 0f;
				decalFadeM = 0f;
				numberPack = NextInt(numberPack, 1);
				for(int i=0;i<allCubes.Count();i++){
					allCubes.ElementAt(i).Key.transform.position = new Vector3(allCubes.ElementAt(i).Value - 10f, allCubes.ElementAt(i).Key.transform.position.y, allCubes.ElementAt(i).Key.transform.position.z);
					allCubes[allCubes.ElementAt(i).Key] = allCubes.ElementAt(i).Key.transform.position.x;
				}
				allCubes.ElementAt(NextIntCube(numberCube, 3)).Key.transform.position = new Vector3(30f, 13f, 20f);
				allCubes[allCubes.ElementAt(NextIntCube(numberCube, 3)).Key] = allCubes.ElementAt(NextIntCube(numberCube, 3)).Key.transform.position.x;
			}
		}
		if(movinBackward){
			foreach(var elb in allCubes){
				elb.Key.transform.position = Vector3.Lerp(elb.Key.transform.position, new Vector3(elb.Value + 10f, elb.Key.transform.position.y, elb.Key.transform.position.z), Time.deltaTime/speedMove);
			}
			decalFade = Mathf.Lerp(decalFade, x10, Time.deltaTime/speedMove);
			decalFadeM = Mathf.Lerp(decalFadeM, -xm10, Time.deltaTime/speedMove);
			if(Mathf.Abs(allCubes.First().Key.transform.position.x - (allCubes.First().Value + 10f)) <= limite){
				movinBackward = false;	
				decalFade = 0f;
				decalFadeM = 0f;
				numberPack = PrevInt(numberPack, 1);
				for(int i=0;i<allCubes.Count();i++){
					allCubes.ElementAt(i).Key.transform.position = new Vector3(allCubes.ElementAt(i).Value + 10f, allCubes.ElementAt(i).Key.transform.position.y, allCubes.ElementAt(i).Key.transform.position.z);
					allCubes[allCubes.ElementAt(i).Key] = allCubes.ElementAt(i).Key.transform.position.x;
				}
				allCubes.ElementAt(PrevIntCube(numberCube, 3)).Key.transform.position = new Vector3(-30f, 13f, 20f);
				allCubes[allCubes.ElementAt(PrevIntCube(numberCube, 3)).Key] = allCubes.ElementAt(PrevIntCube(numberCube, 3)).Key.transform.position.x;
			}
		}
	}
	
	
	void createPacks(){
		allCubes = new Dictionary<GameObject, float>();
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(-30f, 13f, 20f), miniCubePack.transform.rotation), -30f);
		allCubes.Last().Key.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[LoadManager.Instance.ListSong().ElementAt(0).Key];
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(-20f, 13f, 20f), miniCubePack.transform.rotation), -20f);
		allCubes.Last().Key.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[LoadManager.Instance.ListSong().ElementAt(1).Key];
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(-10f, 13f, 20f), miniCubePack.transform.rotation), -10f);
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(0f, 13f, 20f), miniCubePack.transform.rotation), 0f);
		allCubes.Last().Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(10f, 13f, 20f), miniCubePack.transform.rotation), 10f);
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(20f, 13f, 20f), miniCubePack.transform.rotation), 20f);
		allCubes.Add((GameObject) Instantiate(miniCubePack, new Vector3(30f, 13f, 20f), miniCubePack.transform.rotation), 30f);
		
	}
	
	
	int NextInt(int i, int recurs){
		var res = 0;
		if(i == (packs.Count - 1)){
			res = 0;	
		}else{
			res = i + 1;
		}
		
		if(recurs > 1){
			return NextInt(res, recurs - 1);
			
		}else{
			return res;
		}
	}
	
	int PrevInt(int i, int recurs){
		var res = 0;
		if(i == 0){
			res = (packs.Count - 1);	
		}else{
			res = i - 1;
		}
		
		if(recurs > 1){
			return PrevInt(res, recurs - 1);	
			
		}else{
			return res;
		}
	}
	
	int NextIntCube(int i, int recurs){
		var res = 0;
		if(i == (allCubes.Count - 1)){
			res = 0;	
		}else{
			res = i + 1;
		}
		
		if(recurs > 1){
			return NextIntCube(res, recurs - 1);
		}else{
			return res;
		}
	}
	
	int PrevIntCube(int i, int recurs){
		var res = 0;
		if(i == 0){
			res = (allCubes.Count - 1);	
		}else{
			res = i - 1;
		}
		
		if(recurs > 1){
			return PrevIntCube(res, recurs - 1);
		}else{
			return res;
		}
	}
}
