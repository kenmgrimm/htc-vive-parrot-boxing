using System.IO;
using UnityEngine;

public class Player : MonoBehaviour {
	// [SerializeField]
	// private TrackedTransform[] trackedTransforms = new TrackedTransform[3];
	
	public string[] names;
	public Transform[] transforms;
	
	private float totalTime = 0f;
	
	private StreamReader streamReader;
	
	void Start () {
		streamReader = new StreamReader("recorder.txt");
		InvokeRepeating("ReplayFrame", 0, 0.011f);
	}
	
	void ReplayFrame () {
 		for(int i = 0; i < names.Length; i++) {
			string line = streamReader.ReadLine();

			if (line == null) {
				streamReader.BaseStream.Position = 0;
				streamReader.DiscardBufferedData(); 
				return;
			}

			char[] fieldTerminators = {':', ',', '|'};
			string[] values = line.Split(fieldTerminators);
			string name = values[0];
			
			Transform matched = transforms[i];
			matched.position = new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
			matched.rotation = 
				new Quaternion(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));
		
		}
	}
	
	void Destroy () {
		streamReader.Close();
	}
}
