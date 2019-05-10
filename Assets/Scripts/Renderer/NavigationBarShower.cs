using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBarShower : MonoBehaviour {

    // https://forum.unity3d.com/threads/disable-immersivemode-unity5.313911/
    void Awake () {
        Screen.fullScreen = false;
    }
}
