using UnityEngine;
 
public class OrientationDebug : MonoBehaviour {
  private float length = 0.5f;
  private Color lightBlue = Color.blue - new Color(0, 0, 0.5f);
  private Color lightRed = Color.red - new Color(0.5f, 0, 0);
  private Color lightGreen = Color.green - new Color(0, 0.5f, 0);
  private Color lightMagenta = Color.magenta - new Color(0.25f, 0.25f, 0.25f);

  public GameObject globalCenter;
  public GameObject localCenter;

  public bool showLocalOrientation = true;
  public bool showGlobalOrientation = true;

  public bool showLocalCenter = true;
  public bool showGlobalCenter = true;

  void Start() {
  }

	void Update() {
    if(showGlobalOrientation) {
      Debug.DrawRay(transform.position, Vector3.forward * length, lightBlue);
      Debug.DrawRay(transform.position, Vector3.up * length, lightGreen);
      Debug.DrawRay(transform.position, Vector3.right * length, lightRed);
    }

    if(showLocalOrientation) {
      Debug.DrawRay(transform.position, transform.forward * length, Color.blue);
      Debug.DrawRay(transform.position, transform.up * length, Color.green);
      Debug.DrawRay(transform.position, transform.right * length, Color.red);
    }

    if(showGlobalCenter) {
      globalCenter.SetActive(true);
      globalCenter.transform.position = transform.position;
    } else {
      globalCenter.SetActive(false);
    }

    if(showLocalCenter) {
      localCenter.SetActive(true);
      localCenter.transform.position = transform.localPosition;
    } else {
      localCenter.SetActive(false);
    }
  }
}