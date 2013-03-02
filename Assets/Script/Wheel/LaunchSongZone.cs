using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LaunchSongZone : MonoBehaviour {
	
	
	//moveToSong
	private Vector2 departSongDiff;
	private Vector2 moveSongDiff;
	public Vector2 arriveSongDiff;
	public float speedMoveSong;
	public Rect posSongTitle;
	public Rect posSubTitle;
	public Rect posArtist;
	public Rect posStepArtist;
	public Rect posBestScore;
	public Rect posTopProfileScore;
	private float[] alphaSongLaunch;
	public float speedAlphaSongLaunch;
	private float alphaBlack;
	public float speedAlphaBlack;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
