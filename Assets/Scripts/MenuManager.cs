using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public void StartLocalGame()
	{
		Application.LoadLevel("LocalGame");
	}
	
	public void ExitGame()
	{
		Application.Quit();
	}
}
