using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : AudioController {

    int songIndex;
    public AudioClip[] songs;

    void Start() {
        Volume = 1;
        songIndex = 0;

        if(songs.Length != 0)
            Play(songs[songIndex]);

        songIndex++;
    }

    void Update() {

        source.volume = Volume;

        if (!isPlayingAudio) {

            songIndex++;

            if (songIndex >= songs.Length)
                songIndex = 0;

            if (songs.Length != 0)
                Play(songs[songIndex]);

        }

    }

}
