using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("OBJECT ENTERED TRIGGER: " + other.gameObject.name);
        if (other.gameObject.name == "Launch Object") {
            WeighingScalesManager.Instance.ShowLevelClearText();
        }
    }
}
