using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour {
    public FadeOut __fadeOut;
    public SceneLoader __sceneLoader;

    public void changeScene(string newScene) {
        __fadeOut.changeScene();
        __sceneLoader.loadScene(newScene);
    }
    public void changeStage(int num) {
        __fadeOut.changeScene();
        __sceneLoader.loadStage(num);
    }
}
