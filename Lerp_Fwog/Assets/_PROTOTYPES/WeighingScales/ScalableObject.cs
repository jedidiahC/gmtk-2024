using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableObject : MonoBehaviour {
    private void OnMouseUpAsButton() {
        Debug.Log("Object clicked: " + gameObject.name);
        HandlerManager.Instance.SetTarget(transform);
    }
}
