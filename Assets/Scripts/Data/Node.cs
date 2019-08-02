using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node {

	public int x;
	public int y;
	public Node next;

    public Vector3 GetVector3() { return new Vector3(x, 0, y); }

    public Node(int a, int b) {
        x = a;
        y = b;
    }
    public Node(Vector2 v) : this((int)v.x, (int)v.y) { }
    public Node(Vector3 v) : this((int)v.x, (int)v.z) { }
    public Node(Obj o) : this(o.X, o.Y) { }

    public T[,] CreateArrayOfSize<T>(){

        T[,] array = new T[x, y];
        return array;

    }

    public override bool Equals(System.Object obj) {
        Node n = obj as Node;

        return x == n.x && y == n.y;
    }

    public override int GetHashCode() {
        return new { x, y }.GetHashCode();
    }

    public float DistanceTo(Node n, bool diagonal) {

		float dx = Mathf.Abs(x - n.x);
		float dy = Mathf.Abs(y - n.y);
		float manhattan = dx + dy;

		if (diagonal)
			return 2 * manhattan + 2 * (sqrt2 - 2) * Mathf.Min(dx, dy);

        return manhattan * 2;
    }

	public bool IsDiagonalTo(Node n) {
		
		int dx = Mathf.Abs(x - n.x);
		int dy = Mathf.Abs(y - n.y);
		return dx > 0 && dy > 0;

	}

    public float DistanceTo(Node n) {
        return DistanceTo(n, false);
    }

    public override string ToString() {
        return "(" + x + ", " + y + ")";
    }

	public Node GetDirection(Node prev) {

		return new Node(x - prev.x, y - prev.y);

	}

	public Node GetLeftFromHere(Node direction) {

		if (direction == null)
			Debug.LogError("Direction node does not exist");

		if (direction.Equals(west))
			return new Node(x - 1, y); //west

		else if (direction.Equals(north))
			return new Node(x, y + 1); //north

		else if (direction.Equals(east))
			return new Node(x + 1, y); //east

		else if (direction.Equals(south))
			return new Node(x, y - 1); //south

		return null;

	}

	public Node GetRightFromHere(Node direction) {

		if (direction == null)
			Debug.LogError("Direction node does not exist");

		if (direction.Equals(west))
			return new Node(x + 1, y); //west

		else if (direction.Equals(north))
			return new Node(x, y - 1); //north

		else if (direction.Equals(east))
			return new Node(x - 1, y); //east

		else if (direction.Equals(south))
			return new Node(x, y + 1); //south

		return null;

	}

	public Node GetStraightFromHere(Node direction) {

		if (direction == null)
			Debug.LogError("Direction node does not exist");

		if (direction.Equals(west))
			return new Node(x, y + 1); //west

		else if (direction.Equals(north))
			return new Node(x + 1, y); //north

		else if (direction.Equals(east))
			return new Node(x, y - 1); //east

		else if (direction.Equals(south))
			return new Node(x + 1, y); //south

		return null;

	}

	public static Node GetOppositeDirection(Node direction) {

		return new Node(direction.x * -1, direction.y * -1);

	}

	public static float sqrt2 = 1.41421356237f;
	public static Node west = new Node(0, 1);
	public static Node north = new Node(1, 0);
	public static Node east = new Node(0, -1);
	public static Node south = new Node(-1, 0);

	public static Node northwest = new Node(1, 1);
	public static Node northeast = new Node(1, -1);
	public static Node southeast = new Node(-1, -1);
	public static Node southwest = new Node(-1, 1);

}

[System.Serializable]
public class Float3d {

	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }

    public Float3d(float x, float y, float z) {

		X = x;
		Y = y;
		Z = z;

    }

    public Float3d(Vector3 v) : this(v.x, v.y, v.z) { }

    public Float3d(Color c) : this(c.r, c.g, c.b) { }

    public override bool Equals(System.Object obj) {
        Float3d n = obj as Float3d;

        return X == n.X && Y == n.Y && Z == n.Z;
    }

    public override int GetHashCode() {
        return new { X, Y, Z }.GetHashCode();
    }

    public Vector3 GetVector3() { return new Vector3(X, Y, Z); }
    public Color GetColor() { return new Color(X, Y, Z); }

}