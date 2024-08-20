using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class FinaleFrog : MonoBehaviour {
    [SerializeField] private Rigidbody2D _rigidbody;

    void Awake() {
        Debug.Assert(_rigidbody != null, "_rigidbody is not assigned");
    }

    public void SetFrogScale(int inScore) {
        const float BASE_SCALE = 0.75f;
        const float SCALE_FACTOR = 0.005f;
        float scaledValue = Mathf.Log(Mathf.Max(0, inScore - 500) * SCALE_FACTOR + 1) * 0.5f;

        transform.localScale = Vector3.one * (BASE_SCALE + scaledValue);
    }
}
