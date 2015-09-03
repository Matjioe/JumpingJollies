using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MenuNetworkManager : NetworkManager {
	
	public void StartGame()
	{
		NetworkManager.singleton.StartHost();
	}

	public void JoinGame()
	{
		NetworkManager.singleton.StartClient();
	}
}
