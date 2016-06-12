using System.IO;
using UnityEngine;

// Disabling a script only turns off Start & Update (plus related such as FixedUpdate) and OnGUI, 
//  so if those functions aren't present then disabling a script isn't possible.
public class Recorder : MonoBehaviour {
	public string[] names;
	public float totalTime = 0f;
	public Transform[] trackedTransforms;
	public Transform[] parrotingTransforms;
	
	private StreamWriter streamWriter;

	void Start () {
	  streamWriter = new StreamWriter("recorder.txt");
		
		GameObject.FindGameObjectWithTag("Head").SetActive(false);
		GameObject.FindGameObjectWithTag("Left Fist").SetActive(false);
		GameObject.FindGameObjectWithTag("Right Fist").SetActive(false);
		
		InvokeRepeating("RecordFrame", 10, 0.011f);
	}
	
	void RecordFrame() {
		print(Time.deltaTime);
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
