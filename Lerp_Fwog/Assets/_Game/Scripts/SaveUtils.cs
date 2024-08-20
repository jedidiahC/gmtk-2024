using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveUtils {
    private const string LEVEL_SCORES_KEY = "LevelScores";

    public static void SaveScores(int[] inScores) {
        string scoreString = string.Join(",", inScores);
        PlayerPrefs.SetString(LEVEL_SCORES_KEY, scoreString);
        PlayerPrefs.Save();
    }

    public static int[] LoadScores() {
        string scoreString = PlayerPrefs.GetString(LEVEL_SCORES_KEY, "");

        if (string.IsNullOrEmpty(scoreString)) {
            return new int[Constants.NUM_LEVELS];
        }

        string[] scoreStrings = scoreString.Split(',');
        int[] scores = new int[scoreStrings.Length];
        for (int i = 0; i < scoreStrings.Length; i++) {
            int.TryParse(scoreStrings[i], out scores[i]);
        }

        return scores;
    }

    public static void DeleteAllData() {
        PlayerPrefs.DeleteAll();
    }
}
