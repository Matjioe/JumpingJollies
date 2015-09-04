using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public void StartLocalGame()
	{
		Application.LoadLevel("LocalGame");
	}

	public void LoadLobby()
	{
		// Destory network manager because we will use the one from the lobby scene 
		Destroy(FindObjectOfType<MenuNetworkManager>());
		Application.LoadLevel("LobbyScene");
	}
	
	public void ExitGame()
	{
		Application.Quit();
	}
}
