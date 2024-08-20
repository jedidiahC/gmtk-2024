using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinaleManager : MonoBehaviour
{
    [SerializeField] private List<FinaleFrog> _finaleFrogs;
    [SerializeField] private AstroFrog _astroFrog = null; 

    void Awake() {
        Debug.Assert(_finaleFrogs != null && _finaleFrogs.Count == (Constants.NUM_LEVELS - 1), "_finaleFrogs is not assigned!, Double check correct number of FinaleFrogs");
        Debug.Assert(_astroFrog != null, "_astroFrog is not assigned!");


        int[] scores = SaveUtils.LoadScores();
        for (int i = 0; i < (Constants.NUM_LEVELS - 1); i++) {
            _finaleFrogs[i].SetFrogScale(scores[i]);
            Debug.Log(scores[i]);
        }
    }

    void Update() {
        const float MAX_FROG_DISTANCE_Y = 5.0f;
        Vector2 camFromFrog = _astroFrog.transform.position - Camera.main.transform.position;
        Debug.Log(camFromFrog);
        if (camFromFrog.y > MAX_FROG_DISTANCE_Y) {
            Camera.main.transform.position += Vector3.up * (camFromFrog.y - MAX_FROG_DISTANCE_Y);
        } else if (camFromFrog.y < -MAX_FROG_DISTANCE_Y) {
            Camera.main.transform.position += Vector3.up * (camFromFrog.y + MAX_FROG_DISTANCE_Y);
        }

        const float MAX_FROG_DISTANCE_X = 15.0f;
        if (camFromFrog.x > MAX_FROG_DISTANCE_X) {
            Camera.main.transform.position += Vector3.right * (camFromFrog.x - MAX_FROG_DISTANCE_X);
        } else if (camFromFrog.x < -MAX_FROG_DISTANCE_X) {
            Camera.main.transform.position += Vector3.right * (camFromFrog.x + MAX_FROG_DISTANCE_X);
        }
    }
}
