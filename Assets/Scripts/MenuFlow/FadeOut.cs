using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {
    private Animator myFadeOutAnimator;
    
    void Start() {
        myFadeOutAnimator = GetComponent<Animator>();
    }

    public void changeScene() {
        myFadeOutAnimator.SetTrigger("sceneChange");
    }
}
