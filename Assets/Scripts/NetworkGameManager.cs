using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkGameManager : NetworkBehaviour {

	[SyncVar]
	public float RPS = 0.0f;
	[SyncVar]
	public float pitch = 1.0f;
	
	void Update () {
		if (!isServer)
			return;
		
		RPS = GameManager.instance.UpdatePoleRotation(Time.deltaTime);
	}
}
