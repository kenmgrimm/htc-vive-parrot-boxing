using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class Movement {
  private static string[] MOVEMENT_TYPES = { "jab1", "cross1", "dodge_left", "dodge_right", "left_hook", "right_hook",
    "left_uppercut", "right_uppercut", "left_guard", "right_guard", "guard_cheeks", "guard_face", "hand to face" };

	public List<OpponentAction> Actions { get; set; }

  private static Dictionary<string, Movement> movements;

	private static char[] FIELD_TERMINATORS = {':', ',', '|'};

  public static void LoadAnimations() {
    movements = new Dictionary<string, Movement>();
    foreach (string movementName in MOVEMENT_TYPES) {
      movements[movementName] = new Movement(movementName);
    }
  }

  public Movement(string name) {
    StreamReader streamReader = new StreamReader("Assets/Scripts/Action Animations/" + name + ".txt");
		Actions = new List<OpponentAction>();

		while(!streamReader.EndOfStream) {
			TransformOrientation head = ProcessLine(streamReader.ReadLine());
			TransformOrientation leftHand = ProcessLine(streamReader.ReadLine());
			TransformOrientation rightHand = ProcessLine(streamReader.ReadLine());

		 	Actions.Add(new OpponentAction(head, leftHand, rightHand));
		}
		
		streamReader.Close();
  }

	public OpponentAction InitialOrientation() {
		return Actions[0];
	}

	private TransformOrientation ProcessLine(string line) {
		string[] values = line.Split(FIELD_TERMINATORS);

		Vector3 position = new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
		Quaternion rotation = new Quaternion(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));

		return new TransformOrientation(position, rotation);
	}
}


public class OpponentAction {
	public TransformOrientation head;
	public TransformOrientation leftHand;
	public TransformOrientation rightHand;

	public OpponentAction(TransformOrientation head, TransformOrientation leftHand, TransformOrientation rightHand) {
		this.head = head;
		this.leftHand = leftHand;
		this.rightHand = rightHand;
	}
}

