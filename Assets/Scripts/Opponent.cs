using System.IO;
using UnityEngine;

// Disabling a script only turns off Start & Update (plus related such as FixedUpdate) and OnGUI, 
//  so if those functions aren't present then disabling a script isn't possible.
public class Opponent : MonoBehaviour {
	// [SerializeField]
	private float TARGET_DISTANCE = 1.25f;

	// [SerializeField]
	private float DISTANCE_RANGE = 0.1f;

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

  private Vector3 mockPlayerStartPosition = new Vector3(-1f, 0.83f, 0);
	private Quaternion mockPlayerStartRotation = Quaternion.Euler(new Vector3(0, 0, 0));
	
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
		Vector3 playerGroundPos = player.position;
		playerGroundPos.y = 0;
		Vector3 oppGroundPos = opponent.transform.position;
		oppGroundPos.y = 0;

		Vector3 towardsPlayer = playerGroundPos - oppGroundPos;
		// Quaternion lookRotation = Quaternion.Euler(opponent.transform.rotation.eulerAngles);
		Quaternion lookRotation = Quaternion.LookRotation(towardsPlayer.normalized);
	
    float distance = Vector3.Distance(playerGroundPos, oppGroundPos);

		// Point between player and opponent the target distance away from the player
		Vector3 moveTowardsPoint = Vector3.MoveTowards(playerGroundPos, oppGroundPos, TARGET_DISTANCE);
		moveTowardsPoint.y = opponent.transform.position.y;

		Vector3 direction = (oppGroundPos - playerGroundPos).normalized;

    if (TooFar(distance)) {
      opponent.transform.position = Vector3.MoveTowards(opponent.transform.position, moveTowardsPoint, stepSpeed);
		}
		else if (TooClose(distance)) {
			Vector3 newOpponentPos = player.position + direction * stepSpeed;
			newOpponentPos.y = opponent.transform.position.y;
			opponent.transform.position = newOpponentPos;
    }

		lookRotation.x = opponent.transform.rotation.x;
		lookRotation.z = opponent.transform.rotation.z;
		opponent.transform.rotation = lookRotation;
		// opponent.transform.rotation = Quaternion.Slerp(opponent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

		if (log) {
			print(playerGroundPos);
			print(opponent.transform.rotation);
			print(lookRotation);
			print(lookRotation.eulerAngles);
			print(opponent.transform.rotation);
			print(Quaternion.Slerp(opponent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed));	
		}
	}

	private bool TooFar(float distance) {
		print("Too Far: " + distance + ": " + (distance - TARGET_DISTANCE > DISTANCE_RANGE));
		return distance - TARGET_DISTANCE > DISTANCE_RANGE;
	}

	private bool TooClose(float distance) {
		print("Too Close: " + distance + ": " + (TARGET_DISTANCE - distance > DISTANCE_RANGE));
		return TARGET_DISTANCE - distance > DISTANCE_RANGE;
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
