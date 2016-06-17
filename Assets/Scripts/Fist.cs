using UnityEngine;

public class Fist : MonoBehaviour {
	void Start () {
		print("starting fist");
	}
	void Update () {}

	void OnCollisionEnter(Collision collision) {
		print("collision?  " + collision.collider.gameObject.name);
		if(collision.collider.gameObject.name == "HeadCollider") {
			print("collision!!!");
		}
	}

	void OnTriggerEnter(Collider other) {
		print("collision?  " + other.gameObject.name);
		if(other.gameObject.name == "HeadCollider") {
			print("collision!!!");
		}
	}
}
