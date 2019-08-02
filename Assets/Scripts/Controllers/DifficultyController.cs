using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour {

    public void ChangeDifficulty(DifficultyLevel lvl) {

        Difficulty.dLevel = lvl;

    }

}
