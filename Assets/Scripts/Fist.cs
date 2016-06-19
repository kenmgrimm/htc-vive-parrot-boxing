using UnityEngine;

public class Fist : MonoBehaviour {
	void Start () {}
	void Update () {}

	void OnTriggerEnter(Collider other) {
		if(NotMe(other) && other.gameObject.name == "HeadCollider") {
			GetComponent<AudioSource>().Play();
			SteamVR_Controller.Input(1).TriggerHapticPulse(100);
		}
	}

	private bool NotMe(Collider other) {
		return !other.gameObject.CompareTag(gameObject.tag);
	}
}
