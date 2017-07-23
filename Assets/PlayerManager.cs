using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Exale.Data;

public class PlayerManager : NetworkBehaviour{

	private Transform assignedSpawnPoint;

	private NetPlayer player;

	[SyncVar(hook = "OnPlayerIdChanged")]
	protected int playerId = -1;

	public int playerNumber {
		get {
			return playerId;
		}
	}

	public bool initialized {
		get;
		private set;
	}

	public PlayerMovement movement {
		get;
		protected set;
	}

	public RoleTypeDefinition playerRoleType {
		get;
		protected set;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnStartClient() {
		base.OnStartClient ();

		if (!initialized && playerId >= 0) {
			Initialize ();
		}
	}

	public void MoveToSpawnLocation (Transform spawnPoint) {
		if (spawnPoint != null) {
			assignedSpawnPoint = spawnPoint;
		}

		movement.Rigidbody.position = assignedSpawnPoint.position;
		movement.transform.position = assignedSpawnPoint.position;

		movement.Rigidbody.rotation = assignedSpawnPoint.rotation;
		movement.transform.rotation = assignedSpawnPoint.rotation;
	}

	private void Initialize() {
		Initialize (NetworkMan.instance.GetPlayerById (playerId));
	}

	private void Initialize(NetPlayer player) {
		if (initialized) {
			return;
		}

		initialized = true;

		this.player = player;
		playerRoleType = RoleLibrary.Instance.GetRoleDataForIndex (player.PlayerRole);

		GameObject playerClassObject = (GameObject)Instantiate (playerRoleType.classPrefab, transform.position, transform.rotation);
		playerClassObject.transform.SetParent (transform, true);

		// display = playerClassObject.GetComponent<PlayerDisplay> ();
		movement = GetComponent<PlayerMovement> ();

		GameManager.AddPlayer (this);

		if (player.hasAuthority) {
			// player.CmdSetReady();
		}
	}

	private void OnPlayerIdChanged (int id) {
		this.playerId = id;
		Initialize ();
	}

	[Server]
	public void SetPlayerId(int id) {
		playerId = id;
	}
}
