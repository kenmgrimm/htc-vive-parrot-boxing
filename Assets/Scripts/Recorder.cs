using System.IO;
using UnityEngine;

public class Recorder : MonoBehaviour {
	// [SerializeField]
	// private TrackedTransform[] trackedTransforms = new TrackedTransform[3];
	
	public string[] names;
	public float totalTime = 0f;
	public Transform[] trackedTransforms;
	public Transform[] parrotingTransforms;
	
	private StreamWriter streamWriter;

	void Start () {
	  streamWriter = new StreamWriter("recorder.txt");
		
		InvokeRepeating("RecordFrame", 0, 0.011f);
	}
	
	void RecordFrame() {
		print(Time.deltaTime);
		for (int i = 0; i < trackedTransforms.Length; i++) {
			Transform tracked = trackedTransforms[i];
			string name = names[i];
			
			parrotingTransforms[i].position = tracked.position;
			parrotingTransforms[i].rotation = tracked.rotation;
			print(name + ':' + 
				tracked.position.x + "," + 
				tracked.position.y + "," +
				tracked.position.z + "|" +
				tracked.rotation.x + "," +
				tracked.rotation.y + "," +
				tracked.rotation.z + "," +
				tracked.rotation.w
				);
				
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
