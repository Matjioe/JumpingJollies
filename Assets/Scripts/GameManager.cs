using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	static public GameManager instance { get { return _instance;} }
	static protected GameManager _instance;

	public Transform[] spawnPositions;
	public Color[] colors;

	// Local player
	public GameObject localPlayerPrefab;
	int localPlayerCount = 0;
	int maxLocalPlayers = 4;
	private GameObject[] localPlayers;
	public string[] controlKeys = {"space", "left shift", "right shift", "left alt"};

	// GameManagers global params
	public float ejectForce = 100.0f;

	// Pole
	public GameObject deathPole;
	public float poleRPS;
	private float lastPoleRPS;
	private float startPoleRPS;
	private float timeSpent;
	//public float RPSIncrement = 0.01f;
	public AnimationCurve speedCurve;

	// Music
	public UnityEngine.Audio.AudioMixer mixer; // Requires a pitchShifter with pitched exposed param with name "PicthShifter"
	public float pitch;
	public float pitchFactor = 0.1f;

	// NetworkGameManager
	public bool isMultiplayerNetwork = true;
	public NetworkGameManager networkGameManager;

	void Start()
	{
		_instance = this;

		startPoleRPS = poleRPS;
		lastPoleRPS = startPoleRPS;
		timeSpent = 0.0f;

		if (isMultiplayerNetwork == false)
		{
			localPlayers = new GameObject[maxLocalPlayers];
		}
	}

	void Update()
	{
		if (isMultiplayerNetwork)
			return; // Managed by NetworkGameManager

		GameManager.instance.UpdatePoleRotation(Time.deltaTime);
	}

	public Transform GetSpawnPosition(uint playerNb)
	{
		int idx = (int)playerNb % spawnPositions.Length;
		return spawnPositions[idx];
	}

	public Color GetColor(uint playerNb)
	{
		int idx = (int)playerNb % spawnPositions.Length;
		return colors[idx];
	}

	public string GetControlKey(uint playerNb)
	{
		int idx = (int)playerNb % controlKeys.Length;
		return controlKeys[idx];
	}

	public void TriggerPlayerJump(int playerNb)
	{
		if (playerNb >= localPlayerCount || localPlayers[playerNb] == null)
		{
			Debug.LogError("Bug!");
			return;
		}

		localPlayers[playerNb].GetComponent<Player>().TriggerJump();
	}

	// Return false if cannot add any more player
	public bool AddLocalPlayer()
	{
		if (localPlayerCount >= maxLocalPlayers)
			return false;

		uint playerNb = (uint)localPlayerCount;
		Transform spawnPosition = GetSpawnPosition(playerNb);
		localPlayers[localPlayerCount] = (GameObject) Instantiate(localPlayerPrefab, spawnPosition.position, spawnPosition.rotation);
		Player player = localPlayers[localPlayerCount].GetComponent<Player>();
		player.playerNb = playerNb;
		player.isMultiplayerNetwork = false;
		localPlayerCount++;
		return true;
	}

	public bool IsLocalPlayerCountReached()
	{
		if (localPlayerCount >= maxLocalPlayers)
			return true;

		return false;
	}

	public int GetPlayerCount()
	{
		return localPlayerCount;
	}

	public void RemoveAllLocalPlayers()
	{
		for (int i = 0; i < localPlayerCount; ++i)
		{
			Destroy(localPlayers[i]);
		}
		localPlayerCount = 0;
	}

	public float GetPoleRPS()
	{
		if (isMultiplayerNetwork)
			return networkGameManager.RPS;
		else
			return poleRPS;
	}

	public float UpdatePoleRotation(float deltaTime)
	{
		if (deathPole == null)
			Debug.LogError("Missing pole instance!");

		deathPole.transform.Rotate(Vector3.up * deltaTime * poleRPS * 360);
		//RPS = (RPS+RPSIncrement*Time.deltaTime);
		
		// Curve evaluation
		poleRPS = -speedCurve.Evaluate (timeSpent);
		timeSpent += deltaTime;
		
		return poleRPS;
	}

	// Return true if pitch changed
	public bool UpdatePitchShifter()
	{
		if (poleRPS - lastPoleRPS > 0.1f) {
			lastPoleRPS = poleRPS;
			pitch = 1.0f + (poleRPS - startPoleRPS) * pitchFactor;
			return true;
		}

		return false;
	}

	void UpdatePitch()
	{
		mixer.SetFloat ("PitchShifter", pitch);
	}
}
