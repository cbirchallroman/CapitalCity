using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireman : Actor {

    public override void OnceAtDestination() {

        Destination.TurnToRubble();

    }

}
