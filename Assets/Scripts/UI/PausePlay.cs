using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePlay : MonoBehaviour {

    public GameObject pause;
    public GameObject play;
    public Slider timeSlider;

    float previous;

	public void Pause() {

        previous = timeSlider.value;
        timeSlider.value = 0;
        UpdateButton();

    }

    public void Play() {

        timeSlider.value = previous;
        UpdateButton();

    }

    public void UpdateButton() {

        play.SetActive(timeSlider.value == 0);
        pause.SetActive(timeSlider.value != 0);

    }

}
