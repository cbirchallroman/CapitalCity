using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public float Volume { get; set; }
    public AudioSource source;

    private void Start() {

        Volume = 1;

    }

    private void Update() {
        
        source.volume = Volume;

    }

    public void Play(string s) {

        AudioClip clip = Resources.Load<AudioClip>("Audio/" + s);
        Play(clip);

    }

    public void Play(AudioClip clip) {

        source.Stop();
        source.PlayOneShot(clip);

    }

    public bool isPlayingAudio { get { return source.isPlaying; } }

}
