using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour {

	private Color selectedColor = new Color(0f, 1f, 0f, 1f);
	private Color deselectedColor = new Color(1f, 1f, 1f, 1f);
	private Color notReadyColor = new Color(1f, 0.35f, 0.5f, 1f);
	private Color readyColor = new Color(0.35f, 1f, 0.5f, 1f);

	[SerializeField]
	protected InputField nameInput;
	[SerializeField]
	protected Button predatorButton;
	[SerializeField]
	protected Button preyButton;
	[SerializeField]
	protected Button readyButton;

	private NetworkMan netManager;
	private NetPlayer netPlayer;

	public void Init(NetPlayer player) {
		this.netPlayer = player;
		if (netPlayer != null) {
			netPlayer.syncVarsChanged += OnNetworkPlayerSyncvarChanged;
		}

		netManager = NetworkMan.instance;

		MainMenuUI mainMenu = MainMenuUI.Instance;

		mainMenu.playerList.AddPlayer (this);

		if (netPlayer.hasAuthority) {
			SetupLocalPlayer ();
		} else {
			SetupRemotePlayer ();
		}

		UpdateValues ();
	}

	public void SelectPredatorButton() {
		predatorButton.image.color = selectedColor;
		preyButton.image.color = deselectedColor;
	}

	public void SelectPreyButton() {
		predatorButton.image.color = deselectedColor;
		preyButton.image.color = selectedColor;
	}

	public void SetReadyButtonColor(bool ready) {
		readyButton.image.color = ready ? readyColor : notReadyColor;
	}

	public void OnRolechangeClicked(int role) {
		if (role == 0 && !netManager.canSelectPredator) {
			return;
		}

		netPlayer.CmdChangeRole (role);

		if (PlayerDataManager.InstanceExists) {
			if (role == 0) {
				SelectPredatorButton ();
			}
		}
	}

	public void OnNameChanged(string str) {
		netPlayer.CmdNameChanged (str);
	}

	public void OnReadyClicked() {
		netPlayer.CmdSetReady ();
	}

	private void OnNetworkPlayerSyncvarChanged(NetPlayer player) {
		UpdateValues ();
	}

	private void SetupLocalPlayer() {
		nameInput.interactable = true;
		nameInput.image.enabled = true;

		nameInput.onEndEdit.RemoveAllListeners ();
		nameInput.onEndEdit.AddListener (OnNameChanged);
	}

	private void SetupRemotePlayer() {
		DeactivateInteractables ();
	}

	private void DeactivateInteractables() {
		nameInput.interactable = false;
		nameInput.image.enabled = false;
		predatorButton.interactable = false;
		preyButton.interactable = false;
		readyButton.interactable = false;
	}

	private void UpdateValues() {
		nameInput.text = netPlayer.PlayerName;
		SetReadyButtonColor (netPlayer.Ready);

		if (netPlayer.PlayerRole != -1) {
			if (netPlayer.PlayerRole == 0) {
				SelectPredatorButton ();
			} else if (netPlayer.PlayerRole == 1) {
				SelectPreyButton ();
			}
		}
	}
}
