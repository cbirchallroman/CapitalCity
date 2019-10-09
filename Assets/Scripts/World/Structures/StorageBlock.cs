using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBlock : MonoBehaviour {

    public List<GameObject> states;

    public void SetState(int state) {

        if (state < 0 || state >= states.Count)
            Debug.LogError("State number for storage block out of bounds");

        for (int i = 0; i < states.Count; i++)
            states[i].SetActive(i == state);

    }

}
