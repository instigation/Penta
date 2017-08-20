using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScoreChanger : MonoBehaviour{
    private GameObject scoreObject = null;

    public IEnumerator changeGradually(int amount) {
        assignObjectIfNotAssigned();
        int originalScore = getScore();
        for(int score = originalScore; score <= originalScore + amount; score++) {
            setScore(score);
            yield return null;
        }
    }
    private void assignObjectIfNotAssigned() {
        if (scoreObject == null)
            scoreObject = gameObject;
    }
    private int getScore() {
        // 형식이 안맞거나 32를 넘으면 위험할 수도
        string score = scoreObject.GetComponent<Text>().text;
        return Convert.ToInt32(score);
    }
    private void setScore(int score) {
        scoreObject.GetComponent<Text>().text = score.ToString();
    }
}
