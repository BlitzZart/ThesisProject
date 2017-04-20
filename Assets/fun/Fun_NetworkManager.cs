using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fun_NetworkManager : NetworkManager {

    public bool isClient = false;

	void Start () {
		if (isClient) {
            StartClient();
        } else {
            StartServer();
        }
	}
	
	void Update () {
		
	}
}
