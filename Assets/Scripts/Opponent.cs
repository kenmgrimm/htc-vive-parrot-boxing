using System.IO;
using UnityEngine;

// Disabling a script only turns off Start & Update (plus related such as FixedUpdate) and OnGUI, 
//  so if those functions aren't present then disabling a script isn't possible.
public class Opponent : MonoBehaviour {
	[SerializeField]
	private bool rotate = false;

	[SerializeField]
	private float rotationSpeed = 1.0f;
	[SerializeField]
	private float stepSpeed = 1.0f;
	
	[SerializeField]
	private string[] trackedTransformNames;
	[SerializeField]
	private Transform[] avatarTransforms;

  [SerializeField]
  private Transform player;
  [SerializeField]
  private GameObject mockPlayerPrefab;

  [SerializeField]
  private bool mockPlayer = true;
	
	private TransformOrientation[] previousTransformOrientations;
	
	private StreamReader streamReader;
	
	private GameObject opponent;
	private GameObject body;
	
	// Eventually these should be imported as the body transform
	private Vector3 opponentStartPosition = new Vector3(0.933f, 0, 0);
	private Quaternion opponentStartRotation = Quaternion.Euler(new Vector3(0, 270, 0));

  private Vector3 mockPlayerStartPosition = new Vector3(-1f, 0.5f, 0);
	private Quaternion mockPlayerStartRotation = Quaternion.Euler(new Vector3(0, 180, 0));
	
	void Start () {
		// this.enabled = false;
		// return;
		
		previousTransformOrientations = new TransformOrientation[avatarTransforms.Length];
		
    if(mockPlayer) {
      player = (
        GameObject.Instantiate(mockPlayerPrefab, mockPlayerStartPosition, mockPlayerStartRotation) as GameObject
      ).transform;
    }

		opponent = GameObject.FindGameObjectWithTag("Avatar");		
		body = GameObject.Find("Capsule");
		
		streamReader = new StreamReader("recorder2.txt");
		
    InvokeRepeating("MoveOpponent", 1, 1);

		InvokeRepeating("ReplayFrame", 0, 0.011f);
	}

	void Update () {
	}
	
  void MoveOpponent() {
		Vector3 playerPos = player.position;
		Vector3 oppPos = opponent.transform.position;
		Vector3 towardsPlayer = playerPos - oppPos;
		Quaternion lookRotation = Quaternion.LookRotation(towardsPlayer.normalized);
	

    float distance = Vector3.Distance(player.position, opponent.transform.position);
    if (distance > 1 || distance < 0.5) {
      opponent.transform.position = Vector3.MoveTowards(opponent.transform.position, player.position, 0.15f);
    }

		print(playerPos);
		print(opponent.transform.rotation);
		print(lookRotation);
		print(lookRotation.eulerAngles);
		print(Quaternion.Slerp(opponent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed));
		opponent.transform.rotation = lookRotation;
			// Quaternion.Slerp(opponent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
		print(opponent.transform.rotation);	
  }

	void ReplayFrame () {
		char[] fieldTerminators = {':', ',', '|'};
		
		if(rotate) {
			opponent.transform.RotateAround(body.transform.position, Vector3.up, 0.25f);
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
			string name = values[0];
			
			Vector3 currentPosition = new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
			Quaternion currentRotation = new Quaternion(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));

			// First frame played for this controller
			if (previousTransformOrientations[i] == null) {
        previousTransformOrientations[i] = new TransformOrientation(currentPosition, currentRotation);
				
        // Move avatar to starting location)
        if (i == 0) {  // first frame
          opponent.transform.position = opponentStartPosition;
          opponent.transform.rotation = opponentStartRotation;
				}
      }
		
			// Print all positions / rotations relative to body to help us locally position inside Avatar	
			// print(this.avatarTransforms[0].position - this.body.transform.position);
			// print((Quaternion.Inverse(this.body.transform.rotation) * this.avatarTransforms[0].rotation).eulerAngles);
			
			// print(this.avatarTransforms[1].position - this.body.transform.position);
			// print((Quaternion.Inverse(this.body.transform.rotation) * this.avatarTransforms[1].rotation).eulerAngles);
			
			// print(this.avatarTransforms[2].position - this.body.transform.position);
			// print((Quaternion.Inverse(this.body.transform.rotation) * this.avatarTransforms[2].rotation).eulerAngles);
			
			TransformOrientation previousOrientation = previousTransformOrientations[i];	

			// Debug.Log(name);
			// Debug.Log("previousOrientation.rotation: " + previousOrientation.rotation.ToString("F4"));
			// Debug.Log("current.rotation: " + currentRotation.ToString("F4"));
			
			Vector3 velocity = currentPosition - previousOrientation.position;
			
			// Find the relative rotation between Quaternion A and B: Quaternion.Inverse(a) * b; 
			Quaternion rotation = Quaternion.Inverse(previousOrientation.rotation) * currentRotation;
			// Vector3 rotation = currentRotation.eulerAngles - previousOrientation.rotation.eulerAngles;
			// Quaternion rotation = Quaternion.FromToRotation(previousOrientation.rotation.eulerAngles, currentRotation.eulerAngles);
			// Debug.Log("Velocity: " + velocity.ToString("F4"));
			// Debug.Log("Rotation: " + rotation.ToString("F4"));
			
			controller.localPosition += velocity;
			
			// Move body under head
			if (i == 0) {
				body.transform.localPosition = controller.localPosition - new Vector3(0, 0.5f, 0);	
				body.transform.localRotation = Quaternion.identity;
			}
			
			controller.localRotation *= rotation;
			
			previousTransformOrientations[i] = new TransformOrientation(currentPosition, currentRotation);
			
			// Debug.Log("Set previous to: " + previousTransformOrientations[i].position.ToString("F4"));
			// Debug.Log("Set previous to: " + previousTransformOrientations[i].rotation.ToString("F4"));
		}
		
	}
	
	void Destroy () {
		streamReader.Close();
	}

}
