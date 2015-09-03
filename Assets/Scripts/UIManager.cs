using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject addPlayerButton;
	public GameObject singlePlayerButton;
	public GameObject[] multiPlayerButtons;

	void Start()
	{
		singlePlayerButton.SetActive(false);
		DeactivateAllMultiplayerButtons();
	}

	public void AddPlayer()
	{
		GameManager.instance.AddLocalPlayer();
		if (GameManager.instance.IsLocalPlayerCountReached())
		{
			addPlayerButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
		}

		int playerCount = GameManager.instance.GetPlayerCount();
		if (playerCount == 1)
		{
			singlePlayerButton.SetActive(true);
		}
		else
		{
			if (playerCount == 2)
				multiPlayerButtons[0].SetActive(true); // We go from single to multicontrol, so we need to enable the multiplayer control for player 1
			singlePlayerButton.SetActive(false);
			multiPlayerButtons[playerCount-1].SetActive(true);
		}
	}

	public void RemoveAllPlayers()
	{
		GameManager.instance.RemoveAllLocalPlayers();
		addPlayerButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
		singlePlayerButton.SetActive(false);
		DeactivateAllMultiplayerButtons();
	}

	void DeactivateAllMultiplayerButtons()
	{
		for (int i = 0; i < multiPlayerButtons.Length; ++i)
		{
			multiPlayerButtons[i].SetActive(false);
		}
	}

	public void TriggerPlayerJump(int playerNb)
	{
		if (playerNb < GameManager.instance.GetPlayerCount())
			GameManager.instance.TriggerPlayerJump(playerNb);
	}

	public void BackToMainMenu()
	{
		Destroy(FindObjectOfType<MenuNetworkManager>());
		Application.LoadLevel("MenuScene");
	}
}
