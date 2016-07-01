using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

	public Transform head;
	public Transform leftHand;
	public Transform rightHand;

  [SerializeField]
  private Transform player;
  [SerializeField]
  private GameObject mockPlayerPrefab;

  [SerializeField]
  private bool mockPlayer = true;
	
	private Movement jabCrossUpper;
	private IEnumerator actionIterator;

	private bool firstFrame = true;
	private Dictionary<string, TransformOrientation> previous;
	
	private GameObject opponent;
	private GameObject body;
	
	// Eventually these should be imported as the body transform
	
	private Vector3 opponentStartPosition = new Vector3(0, 0, 6.6f);
	private Quaternion opponentStartRotation = Quaternion.Euler(new Vector3(0, 80, 0));

  private Vector3 mockPlayerStartPosition = new Vector3(-1f, 0.83f, 0);
	private Quaternion mockPlayerStartRotation = Quaternion.Euler(new Vector3(0, 0, 0));
	
	void Start () {
    if(mockPlayer) {
      player = (
        GameObject.Instantiate(mockPlayerPrefab, mockPlayerStartPosition, mockPlayerStartRotation) as GameObject
      ).transform;
    }

		opponent = GameObject.Find("Opponent");

		body = GameObject.Find("Capsule");
		
		jabCrossUpper = new Movement("jab_cross_upper");
		actionIterator = jabCrossUpper.actions.GetEnumerator();
		
    InvokeRepeating("MoveOpponent", 1, 0.75f);
		InvokeRepeating("ReplayFrame", 0, 0.021f);
	}

	void Update () {}
	
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
		return distance - TARGET_DISTANCE > DISTANCE_RANGE;
	}

	private bool TooClose(float distance) {
		return TARGET_DISTANCE - distance > DISTANCE_RANGE;
	}

	void ReplayFrame () {
		if(!actionIterator.MoveNext()) {
			actionIterator.Reset();
			actionIterator.MoveNext();
		}
		OpponentAction action = actionIterator.Current as OpponentAction;
		MoveOpponentTransform(head, action.head);
		MoveOpponentTransform(leftHand, action.leftHand);
		MoveOpponentTransform(rightHand, action.rightHand);	
	}

	void MoveOpponentTransform(Transform controller, TransformOrientation movement) {
		// Keeping track of the previous orientation should not be necessary.  The recorded movement file
		//  should be refactored to have deltas, not absolute positioning and rotation

		// First recorded tick played for this controller
		if (previous == null) {
			previous = new Dictionary<string, TransformOrientation>();

			if (firstFrame) {
				firstFrame = false;
				// Move avatar to starting location)
				opponent.transform.position = opponentStartPosition;
				opponent.transform.rotation = opponentStartRotation;
			}
		}
		if(!previous.ContainsKey(controller.name)) {
			previous[controller.name] = movement;
		}

		TransformOrientation previousOrientation = previous[controller.name];

		Vector3 velocity = movement.position - previousOrientation.position;

		// Find the relative rotation between Quaternion A and B: Quaternion.Inverse(a) * b; 
		Quaternion rotation = Quaternion.Inverse(previousOrientation.rotation) * movement.rotation;

		controller.localPosition += velocity;

		if(controller.name.Equals("Head")) {
			body.transform.localPosition = controller.localPosition - new Vector3(0, 0.5f, 0);
			body.transform.localRotation = Quaternion.identity;
		}
		
		controller.localRotation *= rotation;

		Vector3 previousPosition = new Vector3(movement.position.x, movement.position.y, movement.position.z);
		Quaternion previousRotation = new Quaternion(movement.rotation.x, movement.rotation.y, movement.rotation.z, movement.rotation.w);
		previous[controller.name] = new TransformOrientation(previousPosition, previousRotation);
	}
}
