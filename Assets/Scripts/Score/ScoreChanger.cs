using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScoreChanger : MonoBehaviour{
    public GameObject __scoreObject;
    public GameObject __bestScoreObject;
    private int bestScore;
    private int scoreToChange = 0;

    public void Start()
    {
        int score = GlobalInformation.getOrInitInt("score", 0);
        setScore(score);
        bestScore = GlobalInformation.getOrInitInt("bestScore", 0);
        setBestScore(bestScore);
    }

    public void reset()
    {
        GlobalInformation.setInt("score", 0);
        setScore(0);
    }
    public void changeGradually(int amount)
    {
        if (scoreToChange > 0)
            scoreToChange += amount;
        else
            StartCoroutine(changeGraduallyCoroutine(amount));
    }
    private IEnumerator changeGraduallyCoroutine(int amount) {
        scoreToChange = amount;
        int score = getScore();
        while (scoreToChange > 0)
        {
            score++;
            setScore(score);
            if (score > bestScore)
            {
                setBestScore(score);
            }
            scoreToChange--;
            yield return null;
        }
        GlobalInformation.setInt("score", score);
        GlobalInformation.setInt("bestScore", bestScore);
        yield return null;
    }

    private int getScore() {
        // 형식이 안맞거나 32를 넘으면 위험할 수도
        string score = __scoreObject.GetComponent<Text>().text;
        return Convert.ToInt32(score);
    }
    private void setScore(int score) {
        __scoreObject.GetComponent<Text>().text = score.ToString();
    }
    private void setBestScore(int score)
    {
        bestScore = score;
        __bestScoreObject.GetComponent<Text>().text = "Best score: " + bestScore.ToString();
    }
}
