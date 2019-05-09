using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefControlButton : MonoBehaviour
{
    public void resetPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }
}
