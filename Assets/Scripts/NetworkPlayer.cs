using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class NetworkPlayer : NetworkBehaviour
{
	[SyncVar(hook="OnRagDoll")]
	public bool ragdoll = true;
	[SyncVar(hook="OnKinematic")]
	public bool kinematic = true;

	public Player player; 

	public void Awake()
	{
		InitPlayer();
	}

	void InitPlayer()
	{
		player = GetComponent<Player>();
		player.isMultiplayerNetwork = true;
	}

	public void Start()
	{
		player.InitHead();
	}

	public override void OnStartLocalPlayer ()
	{
		InitPlayer();
		player.playerNb = (uint)base.netId.Value;
		player.InitEjectForce ();
		player.TeleportToSpawnPosition();
		GameManager.instance.RegisterPlayer(player.gameObject);
	}

	// In case the color was set on local GameManager before the client was started.
	public override void OnStartClient ()
	{
		InitPlayer();
		player.playerNb = (uint)base.netId.Value;
		player.SetColor();
	}

	void OnRagDoll(bool newRagDoll)
	{
		ragdoll = newRagDoll;
		player.EnableRagdoll(ragdoll);
	}
	
	void OnKinematic(bool newKinematic)
	{
		kinematic = newKinematic;
		player.SetKinematic(kinematic);
	}

	void Update ()
	{
		if (isClient)
		{
			if (player.IsDead())
			{
				player.Respawn();
				CmdSetRagdoll(false);
				CmdSetKinematic(true);
			}
		}
		
		if (!isLocalPlayer)
			return;
		
		if (player.IsPlaying())
		{
			player.UpdatePlayerController();
			player.ComputeEjectForce();
		}
		else if (player.StartPlaying())
		{
			CmdSetKinematic(false);
			player.OnStartPlaying();
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (!isLocalPlayer)
			return;
		
		if (col.CompareTag ("DeathBar"))
		{
			CmdSetRagdoll(true);
			player.AddEjectForce(GameManager.instance.ejectForce);
		}
	}

	[Command]
	void CmdSetRagdoll(bool enableRagdoll)
	{
		ragdoll = enableRagdoll;
	}
	
	[Command]
	void CmdSetKinematic(bool enableKinematic)
	{
		kinematic = enableKinematic;
	}
}
