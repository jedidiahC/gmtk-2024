using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        ProcessKill(other);
    }
    
    private void OnTriggerStay2D(Collider2D other) {
        ProcessKill(other);
    }

    private void ProcessKill(Collider2D other) {
        if (StageLoader.GetInstance() == null) { // if it's developer testing
            StageManager stageManager = FindObjectOfType<StageManager>();
            if (!stageManager.GetIsSimulating()) return;
        } else {
            StageManager activeStage = StageLoader.GetInstance().GetActiveStage();
            if (activeStage == null || !activeStage.GetIsSimulating()) return;
        }

        if (other.gameObject.tag == Constants.TAG_TARGET_OBJ) {
            other.gameObject.SetActive(false);
        } else if (other.gameObject.GetComponent<Rigidbody2D>() != null) {
            other.gameObject.SetActive(false); // We want the other dynamics to also be killed.
        }
    }
}
