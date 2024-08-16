using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance = null;

    void Awake() {
        if (_instance == null) {
            _instance = this;
            Setup();
        } else if (_instance != null && _instance != this) {
            DestroyImmediate(gameObject);
        }
    }

    void OnDestroy() {
        if (_instance == this) _instance = null;
    }

    private void Setup() {
        // Do setup code here.
    }
}
