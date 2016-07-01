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

	public void BeginRecording() {
		InvokeRepeating("RecordFrame", 0, 0.011f);
	}

	void Start () {
		NewFile();
	}

	private void NewFile() {
		if(streamWriter != null) {
			streamWriter.Close();
		}

		string path = "recording_";
		while(File.Exists(path + lastSequence++ + ".txt")) {}

	  streamWriter = new StreamWriter(path);
	}

	void Update() {
    SteamVR_Controller.Device controller = 
			SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));

		if(controller.GetHairTriggerDown()) {
			NewFile();
		}
	}
	
	void RecordFrame() {
		string line = "";
		
		for (int i = 0; i < trackedTransforms.Length; i++) {
			Transform tracked = trackedTransforms[i];
			string name = names[i];
			
			parrotingTransforms[i].position = tracked.position;
			parrotingTransforms[i].rotation = tracked.rotation;

			line += buildLine(name, tracked);
		}
		
		streamWriter.WriteLine(line);
	}

	private string buildLine(string controllerName, Transform controller) {
		return name + ':' + 
			controller.position.x + "," + 
			controller.position.y + "," +
			controller.position.z + "|" +
			controller.rotation.x + "," +
			controller.rotation.y + "," +
			controller.rotation.z + "," +
			controller.rotation.w + "\n";
	}

	void Destroy () {
		streamWriter.Close();
	}
}
