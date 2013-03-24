using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NetworkWheelScript : MonoBehaviour {
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnApplicationQuit()
	{
		Network.Disconnect();
	}
	
	//server side
	void OnPlayerConnected(NetworkPlayer player)
	{
		
	}
	
	//server side
	void OnPlayerDisconnected(NetworkPlayer player)
	{

	}
	
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		
	}

}
