using UnityEngine;
using System.Collections;

public class OptionControl : MonoBehaviour {
	bool showGUI = true;
	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	void OnGUI() {
		if (showGUI) {
			GUI.Box(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 80), "Click button to change settings, then press start.\n" +
				"Perfect jump allows frogs to only collect-\npellets during landing portion of jump.\n" +
				"Negative jump subtracts points from frogs,-\nforcing them to be more efficient.\n");
			if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2 - 20, 160, 20), "Perfect jump is: " + FrogMove.PerfJump)) {
				FrogMove.PerfJump = !FrogMove.PerfJump;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2, 160, 20), "Negative jump is: " + FrogMove.NegJump)) {
				FrogMove.NegJump = !FrogMove.NegJump;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 20, 160, 20), "Start!")) {
				Application.LoadLevel(1);
			}
		}
	}
}
