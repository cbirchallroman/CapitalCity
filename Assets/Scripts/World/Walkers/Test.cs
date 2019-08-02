using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    

    [Header("Walker")]
    public int lifeTime;
    public int Prevx { get; set; }
    public int Prevy { get; set; }
    public int LaborPoints { get; set; }

    public float MovementDistance { get; set; }
    public float MovementSpeed { get { return 1; } }
    public float MovementTime { get { return MovementDistance / MovementSpeed; } }

    public Structure Origin { get; set; }
    public Structure Destination { get; set; }

    public ItemOrder Order { get; set; }
}
