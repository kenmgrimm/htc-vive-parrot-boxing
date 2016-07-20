using System.IO;
using UnityEngine;

// Disabling a script only turns off Start & Update (plus related such as FixedUpdate) and OnGUI, 
//  so if those functions aren't present then disabling a script isn't possible.
public class Player : MonoBehaviour {
	[SerializeField]
	private bool rotate = false;
	
	[SerializeField]
	private string[] trackedTransformNames;
	[SerializeField]
	private Transform[] avatarTransforms;
	
	private TransformOrientation[] previousTransformOrientations;
	
	private StreamReader streamReader;
	
	private GameObject avatar;
	private GameObject body;
	
	// Eventually these should be imported as the body transform
	private Vector3 avatarStartPosition = new Vector3(0.72f, 0.5f, 0);
	private Quaternion avatarStartRotation = Quaternion.Euler(new Vector3(0, 0, 0));
	
	void Start () {
		if (!this.isActiveAndEnabled) {
			this.enabled = false;
			return;
		} 
		previousTransformOrientations = new TransformOrientation[avatarTransforms.Length];
		
		avatar = GameObject.FindGameObjectWithTag("Avatar");		
		body = GameObject.Find("Capsule");
		
		streamReader = new StreamReader("recorder2.txt");
		
		InvokeRepeating("ReplayFrame", 0, 0.011f);
	}
	
	void ReplayFrame () {
		char[] fieldTerminators = {':', ',', '|'};
		
		if(rotate) {
			avatar.transform.RotateAround(body.transform.position, Vector3.up, 0.25f);	
		}
		
 		for(int i = 0; i < avatarTransforms.Length; i++) {
			Transform controller = avatarTransforms[i];
 
			string line = streamReader.ReadLine();

			if (line == null) {
				streamReader.BaseStream.Position = 0;
				streamReader.DiscardBufferedData();
				
				return;
			}

			string[] values = line.Split(fieldTerminators);
			
			Vector3 currentPosition = new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
			Quaternion currentRotation = new Quaternion(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));

			// First frame played for this controller
			if (previousTransformOrientations[i] == null) {
        previousTransformOrientations[i] = new TransformOrientation(currentPosition, currentRotation);
				
        // Move avatar to starting location)
        if (i == 0) {  // first frame
          avatar.transform.position = avatarStartPosition;
          avatar.transform.rotation = avatarStartRotation;
				}
      }

			TransformOrientation previousOrientation = previousTransformOrientations[i];	

			Vector3 velocity = currentPosition - previousOrientation.position;
			
			// Find the relative rotation between Quaternion A and B: Quaternion.Inverse(a) * b; 
			Quaternion rotation = Quaternion.Inverse(previousOrientation.rotation) * currentRotation;
			
			controller.localPosition += velocity;
			
			// Move body under head
			if (i == 0) {
				body.transform.localPosition = controller.localPosition - new Vector3(0, 0.5f, 0);	
				body.transform.localRotation = Quaternion.identity;
			}
			
			controller.localRotation *= rotation;
			
			previousTransformOrientations[i] = new TransformOrientation(currentPosition, currentRotation);
		}
	}
	
	void Destroy () {
		streamReader.Close();
	}

}
