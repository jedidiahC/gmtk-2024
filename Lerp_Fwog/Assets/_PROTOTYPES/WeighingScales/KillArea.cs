using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Finish") {
            other.gameObject.SetActive(false);
        } else if (other.gameObject.GetComponent<Rigidbody2D>() != null) {
            other.gameObject.SetActive(false); // We want the other dynamics to also be killed.
        }
    }
}
