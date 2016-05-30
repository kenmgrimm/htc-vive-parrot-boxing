using UnityEngine;

public class TransformOrientation {
  public Vector3 position;
  public Quaternion rotation;
  
  public TransformOrientation(Vector3 position, Quaternion rotation) {
    this.position = position;
    this.rotation = rotation;
  }
  
}