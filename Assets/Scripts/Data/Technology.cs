using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Technology {

	public int cost;
    public string name;
    public string prereqItem;
    public Technology prev, next;

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override bool Equals(object obj) {

        Technology io = (Technology)obj;
        return name == io.name;

    }

}
