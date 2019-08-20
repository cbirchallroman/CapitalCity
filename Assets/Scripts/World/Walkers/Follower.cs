using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Walker {

    public Walker Leader { get; set; }

	public override void DoEveryStep() {

		if (Leader == null || !pathfinder.CanGoTo(X, Y, Prevx, Prevy, data))
			DestroySelf();

		UpdateFollowerMovement();

	}

    public void UpdateFollowerMovement() {

        int nextx = Leader.Prevx;
        int nexty = Leader.Prevy;
        transform.position = new Vector3(X, 0, Y);
        Prevx = X;
        Prevy = Y;
        X = nextx;
        Y = nexty;
        Direction = new Node(X - Prevx, Y - Prevy);

        if (!SpawnedFollower)
            SpawnFollower();

        SetDirection();

    }

}
