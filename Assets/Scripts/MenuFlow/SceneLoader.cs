using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public void loadStage(int num) {
        GlobalInformation.setInt("num", num);
        GlobalInformation.setInt("difficulty", (int)Difficulty.HARD);
        loadScene("Stage");
    }
    public void loadScene(string newScene) {
        StartCoroutine("loadSceneAfterSecond", newScene);
    }

    IEnumerator loadSceneAfterSecond(string newScene) {
        yield return new WaitForSeconds(1);
            SceneManager.LoadScene(newScene);
    }
}
