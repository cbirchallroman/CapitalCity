using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WalkerSave : ObjSave {

    public int prevx, prevy, laborPoints, lifeTime, yield;
    public float movementDistance;
    public Node origin, destination, direction;
    public bool stuck, SpawnedFollower;
    public Queue<Node> path;
    public List<string> visitedSpots;
    public ItemOrder order;
    //public Vector3 skin;
    public WalkerSave follower;
    public Float3d skin;
	public WalkerData data;
	public Prole PersonData;

	public WalkerSave(GameObject go) : base(go) {

        Walker w = go.GetComponent<Walker>();

        //save origin and destination as coordinates
        Structure o = w.Origin;
        if (o != null)
            origin = new Node(o.X, o.Y);
        Structure d = w.Destination;
        if (d != null)
            destination = new Node(d.X, d.Y);

        prevx = w.Prevx;
        prevy = w.Prevy;
        laborPoints = w.LaborPoints;
        lifeTime = w.lifeTime;
        yield = w.yield;

        movementDistance = w.MovementDistance;
        
        stuck = w.Stuck;
        SpawnedFollower = w.SpawnedFollower;

        direction = w.Direction;
        order = w.Order;

        path = w.Path;
        visitedSpots = w.VisitedSpots;
        skin = new Float3d(w.Skin);

		data = w.data;
		PersonData = w.PersonData;


		if (w.Follower != null)
            follower = new WalkerSave(w.Follower.gameObject);
    }
}

public class Walker : Obj {

    public float MovementDistance { get; set; }

    [Header("Walker")]
    public int lifeTime;
    public float movementSpeed = 1;
    public int yield;
    public string followerName;
    public int Prevx { get; set; }
    public int Prevy { get; set; }
    public int LaborPoints { get; set; }

    public bool laborSeeker;
    public MeshRenderer meshRenderer;
    
    public bool SpawnedFollower { get; set; }
    public bool Stuck { get; set; }
    public Color Skin { get; set; }
    public Walker Follower { get; set; }
    
    public float MovementTime { get { return MovementDistance / movementSpeed; } }

    public Structure Origin { get; set; }
    public Structure Destination { get; set; }

    public ItemOrder Order { get; set; }

    public Queue<Node> Path { get; set; }
    public List<string> VisitedSpots { get; set; }
    public Node Direction { get; set; }

	public WalkerData data = null;
	public Prole PersonData { get; set; }

	public NaviType preferredRandomNavigation = NaviType.Random;

    public void Move() {
        if (!Stuck) transform.Translate(0, 0, Time.deltaTime * movementSpeed);
    }

    public virtual void DestroySelf() {

        Destroy(gameObject);

    }

	public virtual void Kill() {

		DestroySelf();

	}

    public override void Activate() {
        base.Activate();

		if(string.IsNullOrEmpty(data.name))
			data = WalkerDatabase.GetData(name);

		if (this is Follower) {
            transform.SetParent(world.followers.transform);
        }
            
        else {
            //name = "WalkerFrom_" + Origin.name;
			//THIS MAY OR MAY NOT HAVE BEEN IMPORTANT
            transform.SetParent(world.walkers.transform);
        }
            

        X = (int)transform.position.x;
        Y = (int)transform.position.z;

        Prevx = X;
        Prevy = Y;

        MovementDistance = 1;
        Stuck = true;

        VisitedSpots = new List<string>();

        Skin = PersonData != null ? PersonData.skinColor.GetColor() : GetSkinColor();
        if(meshRenderer != null)
            meshRenderer.material.color = Skin;

		world.EnterSquare(this, X, Y);

	}

    public override void Load(ObjSave o) {
        base.Load(o);

        WalkerSave w = (WalkerSave)o;

		//retrieve origin and destination structures
		if (w.origin != null)
            Origin = GameObject.Find(world.Map.GetBuildingNameAt(w.origin)).GetComponent<Structure>();
        if (w.destination != null)
            Destination = GameObject.Find(world.Map.GetBuildingNameAt(w.destination)).GetComponent<Structure>();
        
        Stuck = w.stuck;
        SpawnedFollower = w.SpawnedFollower;

        Prevx = w.prevx;
        Prevy = w.prevy;
        LaborPoints = w.laborPoints;
        lifeTime = w.lifeTime;
        yield = w.yield;

        MovementDistance = w.movementDistance;

        Path = w.path;
        VisitedSpots = w.visitedSpots;
        Direction = w.direction;
        Order = w.order;

		data = w.data;
		PersonData = w.PersonData;

		Skin = PersonData != null ? PersonData.skinColor.GetColor() : GetSkinColor();
		if (meshRenderer != null)
            meshRenderer.material.color = Skin;

        LoadFollower(w.follower);

		world.EnterSquare(this, X, Y);

	}

	void Update() {

		TimeDelta += Time.deltaTime;

		Move();

		if (TimeDelta > MovementTime) {

			TimeDelta = 0;

			DoEveryStep();

		}

	}

	public virtual void DoEveryStep() {



	}

	public void UpdateRandomMovement() {

		transform.position = new Vector3(X, 0, Y);

		//create potential moves and delete last position if found in list
		List<Node> moves = pathfinder.Neighbors(new Node(X, Y), data);
		moves.Remove(new Node(Prevx, Prevy));
		
		Node next = new Node(X, Y);

		//find node if we have a directional preference
		Node directional = next;
		switch (preferredRandomNavigation) {

			case NaviType.Leftward:
				directional = next.GetLeftFromHere(Direction);
				break;
			case NaviType.Rightward:
				directional = next.GetRightFromHere(Direction);
				break;
			case NaviType.Straight:
				directional = next.GetStraightFromHere(Direction);
				break;

		}

		//if we have a neighbor that meets that preference, use that
		if (moves.Contains(directional)) {
			next = directional;
		}

		//if there are possible moves forward, choose one at random
		else if (moves.Count > 0) {
			int rand = Random.Range(0, moves.Count);
			next = moves[rand];
		}

		//otherwise go backwards
		else if (pathfinder.CanGoTo(Prevx, Prevy, data)) {
			next = new Node(Prevx, Prevy);
		}

		MoveToTile(next);

	}

    public void UpdatePathedMovement() {

		transform.position = new Vector3(X, 0, Y);

		////we shouldn't be on the same tile, but just in case
  //      if (nextx == X && nexty == Y) {

  //          if (Path.Count > 1)
  //              UpdatePathedMovement();
  //          else
  //              Stuck = true;

  //          return;
  //      }

		MoveToTile(Path.Dequeue());

    }

	void MoveToTile(Node n) {

		Prevx = X;
		Prevy = Y;
		X = n.x;
		Y = n.y;

		world.LeaveSquare(this, Prevx, Prevy);
		world.EnterSquare(this, X, Y);

		if (!SpawnedFollower)
			SpawnFollower();

		SetDirection();

	}

    public void SetDirection() {

		Direction = new Node(X - Prevx, Y - Prevy);
		MovementDistance = Mathf.Sqrt(Mathf.Abs(Direction.x) + Mathf.Abs(Direction.y));

        if (MovementDistance == 0)
            MovementDistance = 1;

        Stuck = false;

		//CARDINAL DIRECTIONS
        if (Direction.Equals(Node.west))
			transform.rotation = Quaternion.Euler(0, 0, 0); //west

		else if (Direction.Equals(Node.north))
            transform.rotation = Quaternion.Euler(0, 90, 0); //north

		else if (Direction.Equals(Node.east))
			transform.rotation = Quaternion.Euler(0, 180, 0); //east

		else if (Direction.Equals(Node.south))
			transform.rotation = Quaternion.Euler(0, 270, 0); //south

		//DIAGONAL DIRECTIONS
		else if (Direction.Equals(Node.northwest))
			transform.rotation = Quaternion.Euler(0, 45, 0); //northwest

		else if (Direction.Equals(Node.northeast))
            transform.rotation = Quaternion.Euler(0, 135, 0); //northeast

        else if (Direction.Equals(Node.southeast))
            transform.rotation = Quaternion.Euler(0, 225, 0); //southeast

        else if (Direction.Equals(Node.southwest))
            transform.rotation = Quaternion.Euler(0, 315, 0); //southwest

        else
            Stuck = true;

    }

    public virtual void ReturnHome() {

        List<Node> entrances = Origin.GetAdjRoadTiles();
        if (entrances.Count == 0) {
            DestroySelf();
            return;
        }
        Path = pathfinder.FindPath(new Node(this), entrances, name);

        if (Path.Count == 0)
            DestroySelf();

        data.ReturningHome = true;
        Stuck = true;

    }

    public void LeaveMap() {

        Structure mapExit = GameObject.FindGameObjectWithTag("MapExit").GetComponent<Structure>();
        if (mapExit == null)
            DestroySelf();

        Node start = new Node(this);
        Node end = new Node(mapExit);

        Path = pathfinder.FindPath(start, end, name);
		data.ReturningHome = true;
        Stuck = true;

    }

    public virtual void OnceAtDestination() {

        //do stuff at destination

    }

    public virtual void OnceBackHome() {

        //do stuff back at home location
        DestroySelf();

    }

    public Color GetSkinColor() {

        float start_r = 61f / 255f;
        float start_g = 28f / 255f;
        float start_b = 10f / 255f;
        float diff_r = (229f - 61)/255f;
        float diff_g = (186f - 28)/255f;
        float diff_b = (84f - 10)/255f;

        float percent = Random.Range(0f, 1);
        diff_r *= percent;
        diff_g *= percent;
        diff_b *= percent;

        return new Color(start_r + diff_r, start_g + diff_g, start_b + diff_b);

    }

    public override string GetDescription() {

        return "Walker from " + Origin.DisplayName;

    }

    public void SpawnFollower() {

        if (string.IsNullOrEmpty(followerName))
            return;
        
        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Followers/" + followerName), new Vector3(Prevx, 0, Prevy), transform.rotation);
		go.name = followerName;

        Follower f = go.GetComponent<Follower>();
        SpawnedFollower = true;
        f.Leader = this;
        f.world = world;
        f.Activate();
        f.UpdateFollowerMovement();
        Follower = f;

    }

    public void LoadFollower(WalkerSave w) {

        if (w == null)
            return;

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Followers/" + followerName), new Vector3(Prevx, 0, Prevy), transform.rotation);
		go.name = followerName;

		Follower f = go.GetComponent<Follower>();
        SpawnedFollower = true;
        f.Leader = this;
        f.world = world;
        f.Load(w);
        Follower = f;

    }

    public void OnDestroy() {

        if (Follower == null)
            return;

        Follower.X = X;
        Follower.Y = Y;
        Follower.Direction = Direction = new Node(X - Prevx, Y - Prevy);
        Follower.SetDirection();

    }

	public void FindPathTo(Node end) {

		Path = pathfinder.FindPath(new Node(this), end, data);

	}

	public void FindPathTo(List<Node> ends) {

		Path = pathfinder.FindPath(new Node(this), ends, data);

	}

	public void SetPath(Queue<Node> newPath) {

		Path = newPath;

	}

	public void SellItem(House house, int item, ItemType type, int amountStored) {

		ItemOrder willBuy = house.WillBuy(item, type, amountStored);

		if (willBuy == null)
			return;

		//subtract from house's savings, earn revenue
		house.Savings -= willBuy.ExchangeValue();
		money.GainFromLocalSales(willBuy.ExchangeValue(), willBuy.type);

		//add to house's inventory, subtract from market's inventory
		house.ReceiveItem(willBuy);
		Origin.RemoveItem(willBuy);

	}

	public void SetPersonData(Prole p) {

		PersonData = p;

	}

}
