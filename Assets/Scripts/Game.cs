using UnityEngine;

public class Game : MonoBehaviour {

	[SerializeField]
	private GameObject[] enabledOnRecord;
	[SerializeField]
	private GameObject[] disabledOnRecord;

	[SerializeField]
	private bool record;

	void Start () {
		foreach(GameObject comp in enabledOnRecord) {
			comp.SetActive(record);
		}
		foreach(GameObject comp in disabledOnRecord) {
			comp.SetActive(!record);
		}
	}
	
	void Update () {}
}
