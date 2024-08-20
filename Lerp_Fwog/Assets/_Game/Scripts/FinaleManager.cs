using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinaleManager : MonoBehaviour
{
    [SerializeField] private List<FinaleFrog> _finaleFrogs;
    [SerializeField] private AstroFrog _astroFrog = null; 
    [SerializeField] private CanvasGroup _finaleCanvasGroup = null;
    [SerializeField] private TextMeshProUGUI _totalScoreText = null;
    [SerializeField] private StageManager _stageManager = null;

    private bool _showEnd = false;
    private GameCamera _gameCamera = null;
    
    void Awake() {
        Debug.Assert(_finaleFrogs != null && _finaleFrogs.Count == (Constants.NUM_LEVELS - 1), "_finaleFrogs is not assigned!, Double check correct number of FinaleFrogs");
        Debug.Assert(_astroFrog != null, "_astroFrog is not assigned!");
        Debug.Assert(_finaleCanvasGroup != null, "_finaleCanvasGroup is not assigned!");
        Debug.Assert(_totalScoreText != null, "_totalScoreText is not assigned!");
        Debug.Assert(_stageManager != null, "_stageManager is not assigned!");
        _finaleCanvasGroup.alpha = 0.0f;

        int[] scores = SaveUtils.LoadScores();
        int totalScore = 0;
        for (int i = 0; i < (Constants.NUM_LEVELS - 1); i++) {
            _finaleFrogs[i].SetFrogScale(scores[i]);
            totalScore += scores[i];
        }
        _totalScoreText.text = "Total Score: \n" + totalScore.ToString();

        _stageManager.OnSimulateChange.AddListener(CommitToFinale);

        // NOTE: If this is null, means development mode solo stage.
        _gameCamera = FindObjectOfType<GameCamera>();
    }

    void Update() {
        Vector3 cameraTarget = Camera.main.transform.position;

        const float MAX_FROG_DISTANCE_Y = 5.0f;
        Vector2 camFromFrog = _astroFrog.transform.position - Camera.main.transform.position;
        Debug.Log(camFromFrog);
        if (camFromFrog.y > MAX_FROG_DISTANCE_Y) {
            cameraTarget += Vector3.up * (camFromFrog.y - MAX_FROG_DISTANCE_Y);
        } else if (camFromFrog.y < -MAX_FROG_DISTANCE_Y) {
            cameraTarget += Vector3.up * (camFromFrog.y + MAX_FROG_DISTANCE_Y);
        }

        const float MAX_FROG_DISTANCE_X = 15.0f;
        if (camFromFrog.x > MAX_FROG_DISTANCE_X) {
            cameraTarget += Vector3.right * (camFromFrog.x - MAX_FROG_DISTANCE_X);
        } else if (camFromFrog.x < -MAX_FROG_DISTANCE_X) {
            cameraTarget += Vector3.right * (camFromFrog.x + MAX_FROG_DISTANCE_X);
        }

        if (_gameCamera != null) {
            _gameCamera.SetTargetPos(cameraTarget);
        } else {
            Camera.main.transform.position = cameraTarget;

            // DEBUG
            // int[] score = SaveUtils.LoadScores();
            // for (int i = 0; i < (Constants.NUM_LEVELS - 1); i++) {
            //     score[i] = 501;
            // }
            // SaveUtils.SaveScores(score);
        }

        if (_astroFrog.transform.position.y > 25.0f) {
            ShowEnd();
        }

    }

    void ShowEnd() {
        if (_showEnd) return;

        _showEnd = true;
        StartCoroutine(ShowEndRoutine());
    }

    void Reset() {
        StopAllCoroutines();
        _finaleCanvasGroup.alpha = 0.0f;
        _showEnd = false;
        _astroFrog.StopFireParticles();
    }

    void CommitToFinale() {
        FindObjectOfType<Hud>().gameObject.SetActive(false);
    }

    IEnumerator ShowEndRoutine() {
        _astroFrog.StartFireParticles();
        while (_finaleCanvasGroup.alpha < 1.0f) {
            _finaleCanvasGroup.alpha += Time.deltaTime * 0.25f;
            yield return null;
        }
    }
}

