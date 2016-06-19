using System.IO;
using UnityEngine;

// Disabling a script only turns off Start & Update (plus related such as FixedUpdate) and OnGUI, 
//  so if those functions aren't present then disabling a script isn't possible.
public class Opponent : MonoBehaviour {
	[SerializeField]
	private float TARGET_DISTANCE = 1f;

	[SerializeField]
	private float DISTANCE_RANGE = 0.05f;

	[SerializeField]
	private float rotationSpeed = 1.0f;
	[SerializeField]
	private float stepSpeed = 0.5f;

	[SerializeField]
	private bool log = false;
	
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
	private int lineNumber = 0;
	
	private GameObject opponent;
	private GameObject body;
	
	// Eventually these should be imported as the body transform
	
	private Vector3 opponentStartPosition = new Vector3(0, 0, 6.6f);
	private Quaternion opponentStartRotation = Quaternion.Euler(new Vector3(0, 80, 0));

  private Vector3 mockPlayerStartPosition = new Vector3(-1f, 1, 0);
	private Quaternion mockPlayerStartRotation = Quaternion.Euler(new Vector3(0, 100, 0));
	
	void Start () {
		previousTransformOrientations = new TransformOrientation[avatarTransforms.Length];
		
    if(mockPlayer) {
      player = (
        GameObject.Instantiate(mockPlayerPrefab, mockPlayerStartPosition, mockPlayerStartRotation) as GameObject
      ).transform;
    }

		opponent = GameObject.FindGameObjectWithTag("Avatar");		
		body = GameObject.Find("Capsule");
		
		streamReader = new StreamReader("jab_cross_upper.txt");
		
    InvokeRepeating("MoveOpponent", 1, 0.75f);

		InvokeRepeating("ReplayFrame", 0, 0.021f);
	}

	void Update () {
	}
	
  void MoveOpponent() {
		Vector3 playerPos = player.position;
		Vector3 oppPos = opponent.transform.position;
		Vector3 towardsPlayer = playerPos - oppPos;
		// Quaternion lookRotation = Quaternion.Euler(opponent.transform.rotation.eulerAngles);
		Quaternion lookRotation = Quaternion.LookRotation(towardsPlayer.normalized);
	
    float distance = Vector3.Distance(player.position, opponent.transform.position);

		// Point between player and opponent the target distance away from the player
		Vector3 moveTowardsPoint = Vector3.Lerp(player.position, opponent.transform.position, TARGET_DISTANCE);
		moveTowardsPoint.y = opponent.transform.position.y;
		
		print(Mathf.Abs(distance - TARGET_DISTANCE));

		Vector3 direction = (opponent.transform.position - player.position).normalized;

    if (distance - TARGET_DISTANCE > DISTANCE_RANGE) {
			print(player.position);
			print(opponent.transform.position);
			print(moveTowardsPoint);
			print( Vector3.MoveTowards(opponent.transform.position, moveTowardsPoint, stepSpeed));

      opponent.transform.position = Vector3.MoveTowards(opponent.transform.position, moveTowardsPoint, stepSpeed);
		}
		else if (TARGET_DISTANCE - distance > DISTANCE_RANGE) {
			opponent.transform.position = player.position + direction * stepSpeed;
			// print(opponent.transform.position);
    }

		// lookRotation.y = towardsPlayer.normalized.y;
		lookRotation.x = opponent.transform.rotation.x;
		lookRotation.z = opponent.transform.rotation.z;
		opponent.transform.rotation = lookRotation;
		// opponent.transform.rotation = Quaternion.Slerp(opponent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

		if (log) {
			print(playerPos);
			print(opponent.transform.rotation);
			print(lookRotation);
			print(lookRotation.eulerAngles);
			print(opponent.transform.rotation);
			print(Quaternion.Slerp(opponent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed));	
		}
  }

	void ReplayFrame () {
		char[] fieldTerminators = {':', ',', '|'};
		
 		for(int i = 0; i < avatarTransforms.Length; i++) {
			Transform controller = avatarTransforms[i];
 
			string line = streamReader.ReadLine();
			lineNumber++;
			if (log)
				print(lineNumber);

			if (line == null) {
				streamReader.BaseStream.Position = 0;
				streamReader.DiscardBufferedData();
				lineNumber = 0;

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
