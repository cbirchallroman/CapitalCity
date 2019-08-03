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

//IF I CAN MAKE THIS FUNCTIONAL I CAN REPLACE ALL DICTIONARIES WITH THIS
[System.Serializable]
public class DictObject<T, A> {

	T key;
	A value;

	public DictObject(T k, A v) {

		key = k;
		value = v;

	}

	public A GetValue() {

		return value;

	}

	public void SetValue(A v) {

		value = v;

	}

}