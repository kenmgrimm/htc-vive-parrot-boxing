using System.IO;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	private string[] trackedTransformNames;
	[SerializeField]
	private Transform[] avatarTransforms;
	
	private TransformOrientation[] previousTransformOrientations;
	
	private StreamReader streamReader;
	
	private GameObject avatar;
	private GameObject body;
	
	void Awake () {
		previousTransformOrientations = new TransformOrientation[avatarTransforms.Length];
		
		avatar = GameObject.FindGameObjectWithTag("Avatar");
		body = GameObject.Find("Capsule");
		
		streamReader = new StreamReader("recorder.txt");
		InvokeRepeating("ReplayFrame", 0, 0.011f);
	}
	
	void ReplayFrame () {
		char[] fieldTerminators = {':', ',', '|'};
		
		// avatar.transform.Rotate(Vector3.left);
		
 		for(int i = 0; i < avatarTransforms.Length; i++) {
			Transform controller = avatarTransforms[i];
 
			string line = streamReader.ReadLine();

			if (line == null) {
				streamReader.BaseStream.Position = 0;
				streamReader.DiscardBufferedData(); 
				return;
			}

			string[] values = line.Split(fieldTerminators);
			string name = values[0];
			
			Vector3 currentPosition = new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
			Quaternion currentRotation = new Quaternion(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));

			if (previousTransformOrientations[i] == null) {
				// Move avatar to starting location
				if(i == 0) {
					avatar.transform.position = currentPosition - new Vector3(0, 1, 0);
					avatar.transform.rotation = currentRotation;
				}
				previousTransformOrientations[i] = new TransformOrientation(currentPosition, currentRotation);
				controller.position = currentPosition;
				controller.rotation = currentRotation;
			}
			
			TransformOrientation previousOrientation = previousTransformOrientations[i];	

			Debug.Log(name);
			Debug.Log("previousOrientation.rotation: " + previousOrientation.rotation.ToString("F4"));
			Debug.Log("current.rotation: " + currentRotation.ToString("F4"));
			
			Vector3 velocity = currentPosition - previousOrientation.position;
			
			// Find the relative rotation between Quaternion A and B: Quaternion.Inverse(a) * b; 
			Quaternion rotation = Quaternion.Inverse(previousOrientation.rotation) * currentRotation;
			// Vector3 rotation = currentRotation.eulerAngles - previousOrientation.rotation.eulerAngles;
			// Quaternion rotation = Quaternion.FromToRotation(previousOrientation.rotation.eulerAngles, currentRotation.eulerAngles);
			Debug.Log("Velocity: " + velocity.ToString("F4"));
			Debug.Log("Rotation: " + rotation.ToString("F4"));
			
			controller.position += velocity;
			
			if (i == 0) {
				body.transform.position = controller.position - new Vector3(0, 0.5f, 0);	
				body.transform.rotation = Quaternion.identity;
			}
			
			controller.rotation *= rotation;
			// controller.Rotate(rotation);
			
			previousTransformOrientations[i] = new TransformOrientation(currentPosition, currentRotation);
			
			Debug.Log("Set previous to: " + previousTransformOrientations[i].position.ToString("F4"));
			Debug.Log("Set previous to: " + previousTransformOrientations[i].rotation.ToString("F4"));
		}
	}
	
	void Destroy () {
		streamReader.Close();
	}
	
}
