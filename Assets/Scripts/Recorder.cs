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

	public void BeginRecording() {
		InvokeRepeating("RecordFrame", 0, 0.011f);
	}

	void Start () {
		int seq = 0;
		string path = "recording_0.txt";
		while(File.Exists(path = "recording_" + ++seq + ".txt")) {}

	  streamWriter = new StreamWriter(path);
	}
	
	void RecordFrame() {
		for (int i = 0; i < trackedTransforms.Length; i++) {
			Transform tracked = trackedTransforms[i];
			string name = names[i];
			
			parrotingTransforms[i].position = tracked.position;
			parrotingTransforms[i].rotation = tracked.rotation;

		  streamWriter.WriteLine(name + ':' + 
				tracked.position.x + "," + 
				tracked.position.y + "," +
				tracked.position.z + "|" +
				tracked.rotation.x + "," +
				tracked.rotation.y + "," +
				tracked.rotation.z + "," +
				tracked.rotation.w
				);

		}
	}

	void Destroy () {
		streamWriter.Close();
	}
}
