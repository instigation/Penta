using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    public AudioMixer __masterMixer;
    public Sprite __turnOnImage;
    public Sprite __turnOffImage;
    public readonly float originalVolume = 0;
    private bool isMuted;

    // Start is called before the first frame update
    void Start()
    {
        isMuted = GlobalInformation.getOrInitBool("isMuted", true);
        setGameObjectsAccordingToIsMuted();
    }

    public void toggle()
    {
        isMuted = !isMuted;
        GlobalInformation.setBool("isMuted", isMuted);
        setGameObjectsAccordingToIsMuted();
    }
    private void setGameObjectsAccordingToIsMuted()
    {
        // -80.0dB is mute
        __masterMixer.SetFloat("masterVolume", isMuted ? -80.0f : originalVolume);
        // If it is muted, you could turn it on, not off.
        gameObject.GetComponent<Image>().sprite = isMuted ? __turnOnImage : __turnOffImage;
    }
}
