using UnityEngine;
using UnityEditor;
 
public class OrientationDebug : MonoBehaviour {
  private float length = 0.5f;
  private Color lightBlue = new Color(0.4f, 0.4f, 1f);
  private Color lightRed = new Color(1f, 0.4f, 0.4f);
  private Color lightGreen = new Color(0.4f, 1f, 0.4f);
  private Color lightMagenta = Color.magenta - new Color(0.25f, 0.25f, 0.25f);

  private GameObject globalCenter;
  private GameObject localCenter;

  public bool showLocalOrientation = false;
  public bool showGlobalOrientation = false;

  public bool showLocalCenter = false;
  public bool showGlobalCenter = false;

  void Start() {
    GameObject globalCenterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GlobalCenter.prefab", typeof(GameObject));
    GameObject localCenterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LocalCenter.prefab", typeof(GameObject));

    globalCenter = Instantiate(globalCenterPrefab);
    localCenter = Instantiate(localCenterPrefab);

    globalCenter.name = gameObject.name + " Global Center";
    localCenter.name = gameObject.name + " Local Center";

    globalCenter.SetActive(false);
    localCenter.SetActive(false);
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