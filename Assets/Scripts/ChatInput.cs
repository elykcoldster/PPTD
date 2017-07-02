using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			Image img = GetComponent<Image> ();
			InputField infield = GetComponent<InputField> ();

			if (infield.enabled && infield.text.Length > 0) {
				GameObject.FindObjectOfType<ChatBox>().PushChat (infield.text);
				infield.text = "";
			}

			img.enabled = !img.enabled;
			infield.enabled = !infield.enabled;

			infield.ActivateInputField ();
		}
	}
}
