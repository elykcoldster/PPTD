using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

	public float fadeTime = 2f;
	public bool victory;

	Transform target;
	NetworkPlayer netPlayer;

	float alpha;

	Text gotext;

	// Use this for initialization
	void Start () {
		gotext = GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.instance.victory && netPlayer != null) {
			this.victory = netPlayer.victory;
			string str = this.victory ? "Victory" : "Game Over";
			gotext.text = str;

			alpha += Time.deltaTime / fadeTime;

			Color textcolor = gotext.color;
			Color panelcolor = GetComponent<Image> ().color;
			textcolor.a = alpha;
			panelcolor.a = alpha;

			gotext.color = textcolor;
			GetComponent<Image> ().color = panelcolor;
		}
	}

	public void SetTarget(Transform t) {
		this.target = t;
		this.netPlayer = t.GetComponent<NetworkPlayer> ();
	}

	public void Reset() {
		alpha = 0f;

		Color textcolor = gotext.color;
		Color panelcolor = GetComponent<Image> ().color;
		textcolor.a = 0f;
		panelcolor.a = 0f;

		gotext.color = textcolor;
		GetComponent<Image> ().color = panelcolor;
	}
}
