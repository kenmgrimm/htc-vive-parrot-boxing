using System;
using System.IO;
using UnityEngine;

// Disabling a script only turns off **Start & Update** (plus related such as FixedUpdate) and OnGUI, 
//  so if those functions aren't present then disabling a script isn't possible.

// CRITICAL to have mm precision on beginning of recording and match that in playback opponent
//   positioning
public class Recorder : MonoBehaviour {
	public string[] names;
	public float totalTime = 0f;
	public Transform[] trackedTransforms;
	public Transform[] parrotingTransforms;
	
	private StreamWriter streamWriter;
	private int lastSequence = 0;
	private bool recording = false;

	private SteamVR_Controller.Device left;
	private SteamVR_Controller.Device right;

	private void StartRecording() {
		NewFile();

		InvokeRepeating("RecordFrame", 0, 0.011f);
	}

	private void StopRecording() {
		CloseFile();

		CancelInvoke("RecordFrame");
	}

	void Start () {
		left = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
		right = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
	}

	private void NewFile() {
		string prefix = "recording_";
		string path = "";
		while(File.Exists(path = prefix + lastSequence++ + ".txt")) {}

		Debug.Log("Recording to: " + path);

	  streamWriter = new StreamWriter(path);
	}

	private void CloseFile() {
		if(streamWriter != null) {
			streamWriter.Close();
		}
	}

	void Update() {
		if(left.GetHairTriggerDown() || right.GetHairTriggerDown()) {
			if(!recording) {
				StartRecording();
			}
			else {
				StopRecording();
			}
			recording = !recording;
		}
	}
	
	void RecordFrame() {
		if (!recording) {
			return;
		}
		string line = "";
		
		for (int i = 0; i < trackedTransforms.Length; i++) {
			Transform tracked = trackedTransforms[i];
			string controllerName = names[i];
			
			parrotingTransforms[i].position = tracked.position;
			parrotingTransforms[i].rotation = tracked.rotation;

			line += buildLine(controllerName, tracked);
		}
		
		streamWriter.Write(line);
	}

	private string buildLine(string controllerName, Transform controller) {
		return controllerName + ':' + 
			controller.localPosition.x + "," + 
			controller.localPosition.y + "," +
			controller.localPosition.z + "|" +
			controller.localRotation.x + "," +
			controller.localRotation.y + "," +
			controller.localRotation.z + "," +
			controller.localRotation.w + "\n";
	}

	void OnDestroy () {
		CloseFile();
	}
}
