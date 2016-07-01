using UnityEngine;

// May no longer be necessary, we are starting and stopping recording using trigger
// public class StartTimer : MonoBehaviour {
// 	[SerializeField]
// 	private int secondsRemaining;

// 	private GUIStyle guiStyle;
// 	private Rect rect;
	
// 	void Start() {
//  		guiStyle = new GUIStyle();
 
// 		int w = Screen.width, h = Screen.height;

// 		rect = new Rect(0, 0, w, h * 2 / 100);
// 		guiStyle.alignment = TextAnchor.MiddleCenter;
// 		guiStyle.fontSize = h * 3 / 100;
// 		guiStyle.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);

// 		InvokeRepeating("UpdateTime", 1, 1);
// 	}

// 	void OnGUI() {
// 		if (secondsRemaining > 0) {
// 			GUI.Label(rect, secondsRemaining + "", guiStyle);
// 		}
// 	}

// 	private void StartRecording() {
// 		CancelInvoke("UpdateTime");

// 		gameObject.GetComponent<Recorder>();
// 	}
 
// 	private void UpdateTime() {
// 		if ((secondsRemaining--) == 0) {
// 			StartRecording();
// 		}
// 	}
// }
