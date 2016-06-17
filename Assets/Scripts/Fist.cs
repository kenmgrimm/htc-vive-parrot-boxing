using UnityEngine;

public class Fist : MonoBehaviour {
	void Start () {
		print("starting fist");
	}
	void Update () {}

	void OnTriggerEnter(Collider other) {
		// print("collision?  " + other.gameObject.name);
		if(NotMe(other) && other.gameObject.name == "HeadCollider") {
			print("collision!!!");
			GetComponent<AudioSource>().Play();
		}
	}

	private bool NotMe(Collider other) {
		return !other.CompareTag(gameObject.tag);
	}
}
