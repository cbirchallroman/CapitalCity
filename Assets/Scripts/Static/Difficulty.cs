using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : MonoBehaviour {

    public static DifficultyLevel dLevel = DifficultyLevel.Moderate;

    public static float GetModifier() {

        switch (dLevel) {

            case DifficultyLevel.Easiest:
                return .2f;

            case DifficultyLevel.Easy:
                return .8f;

            case DifficultyLevel.Moderate:
                return 1;

            case DifficultyLevel.Hard:
                return 1.2f;

            case DifficultyLevel.Hardest:
                return 1.5f;

        }

        return 1;
            

    }

}
