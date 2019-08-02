using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DictContainer<T, A> {

    public List<T> keys;
    public List<A> values;

    public DictContainer(Dictionary<T, A> blah) {

        keys = new List<T>();
        values = new List<A>();

        foreach (T key in blah.Keys) {
            
            keys.Add(key);
            values.Add(blah[key]);

        }

    }

    public Dictionary<T, A> GetDictionary() {

        Dictionary<T, A> dict = new Dictionary<T, A>();
        for (int a = 0; a < keys.Count; a++)
            dict.Add(keys[a], values[a]);
        return dict;

    }

}
