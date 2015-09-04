using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Rigidbody ourRigid;

	public uint playerNb = 0;

	//Jumping controls
	public int jumpPower = 10;
	public float groundedDistance = 0.1f;
	public GameObject feet;
	private string controlKey = "space";
	//public bool isGrounded = false;

	public bool isMultiplayerNetwork = false;
	public MyNetworkPlayer networkPlayer = null;
	
	public bool isWaitingForStart = false;

	private Vector3 force; 
	private Vector3 headStartPosition;
	private Quaternion headStartRotation;
	private GameObject head;

	void Start()
	{
		if (isMultiplayerNetwork)
			return; // Managed in NetworkPlayer

		InitHead();
		InitEjectForce ();
		TeleportToSpawnPosition();
		SetColor();
		InitControl();
	}

	void Update ()
	{
		if (isMultiplayerNetwork)
			return; // Managed in NetworkPlayer

		if (IsDead())
		{
			Respawn();
		}
		
		if (IsPlaying())
		{
			UpdatePlayerController();
			ComputeEjectForce();
		}
		else if (StartPlaying())
		{
			OnStartPlaying();
		}
	}
	
	void OnTriggerStay(Collider col)
	{
		if (isMultiplayerNetwork)
			return; // Managed in NetworkPlayer

		if (col.CompareTag ("DeathBar"))
		{
			EnableRagdoll(true);
			AddEjectForce(GameManager.instance.ejectForce);
		}
	}

	public void SetColor()
	{
		Color color = GameManager.instance.GetColor(playerNb);
		SetPrimaryColor(color);
		SetSecondaryColor(color);
	}
	
	void SetPrimaryColor(Color color)
	{
		GetComponent<Renderer>().material.color = color;
	}
	
	void SetSecondaryColor(Color color)
	{
		for (int i = 0; i < transform.childCount; ++i) 
		{
			Renderer rend = transform.GetChild(i).GetComponent<Renderer>();
			if (rend != null)
			{
				rend.material.color = color;
			}
		}
	}
	
	public void TeleportToSpawnPosition()
	{
		Transform spawnTransform = GameManager.instance.GetComponent<GameManager>().GetSpawnPosition(playerNb);
		transform.position = spawnTransform.position;
		transform.rotation = spawnTransform.rotation;
		GetComponent<Rigidbody>().isKinematic = true;
		isWaitingForStart = true;
	}
	
	public void InitHead ()
	{
		headStartPosition = new Vector3 ();
		headStartRotation = new Quaternion ();
		head = transform.FindChild ("Head").gameObject;
		headStartPosition = head.transform.localPosition;
		headStartRotation = head.transform.localRotation;
	}

	void InitControl()
	{
		if (isMultiplayerNetwork)
			return; // Managed in NetworkPlayer
		
		controlKey = GameManager.instance.GetControlKey(playerNb);
	}
	
	public void EnableRagdoll(bool ragdoll)
	{
		if (ragdoll == false)
		{
			ResetBodyPhysics();
			ResetHeadPhysics();
		}
		else
		{
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			transform.FindChild("Head").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		}
	}
	
	public void SetKinematic(bool kinematic)
	{
		GetComponent<Rigidbody>().isKinematic = kinematic;
		head.GetComponent<Rigidbody>().isKinematic = kinematic;
	}

	public bool IsDead()
	{
		if (transform.position.y < -10.0f)
			return true;

		return false;
	}

	public bool IsPlaying()
	{
		return !isWaitingForStart;
	}
	
	public bool StartPlaying()
	{
		if (Input.GetKeyDown (controlKey))
			return true;
		
		return false;
	}

	public void OnStartPlaying()
	{
		SetKinematic(false);
		isWaitingForStart = false;
	}
	
	public void UpdatePlayerController()
	{
		if (Input.GetKeyDown(controlKey))
			Jump();
	}

	public void Jump()
	{
		if (IsGrounded())
			ourRigid.AddForce (0, jumpPower, 0);
	}

	public void TriggerJump()
	{
		if (isWaitingForStart)
		{
			if (isMultiplayerNetwork == true)
				networkPlayer.OnStartPlaying();
			OnStartPlaying();
		}
		else
		{
			Jump ();
		}
	}

	bool IsGrounded()
	{
		return Physics.Raycast(feet.transform.position, Vector3.down, groundedDistance);
	}

	public void InitEjectForce ()
	{
		force = new Vector3 ();
	}

	public void ComputeEjectForce()
	{
		float deathPoleRPS = GameManager.instance.GetPoleRPS ();
		force = -Vector3.Cross ((transform.position - GameManager.instance.deathPole.transform.position), Vector3.up) * deathPoleRPS;
		Debug.DrawLine (transform.position, transform.position + force);
	}

	public void AddEjectForce(float ejectForce)
	{
		GetComponent<Rigidbody>().AddForce(force * ejectForce);
	}

	public void Respawn()
	{
		TeleportToSpawnPosition();
		ResetBodyPhysics();
		ResetHeadPhysics();
	}

	void ResetBodyPhysics()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
	}

	void ResetHeadPhysics()
	{
		head.transform.localPosition = headStartPosition;
		head.transform.localRotation = headStartRotation;
		head.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		head.GetComponent<Rigidbody>().velocity = Vector3.zero;
		head.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; 
	}
}
