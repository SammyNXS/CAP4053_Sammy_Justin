using UnityEngine;
using System.Collections;
using System.Text;

public class AgentScript : MonoBehaviour {

	public float x;
	public float y;
	public float theta;
	public float moveSpeed;
	public float rotateSpeed;
	public bool inControl;
	public float radius;
	public GameObject proj;
	public GameObject[] agents;
	public Pair<double,double>[] adjacentAgents;
	public double upDistance;
	public double leftDistance;
	public double rightDistance;
	public float rangeDistance;

	public class Pair<T, U> {
		public Pair() {
		}
		
		public Pair(T first, U second) {
			this.First = first;
			this.Second = second;
		}
		
		public T First { get; set; }
		public U Second { get; set; }
	};
	
	// Use this for initialization
	public void Start () {
		agents = GameObject.FindGameObjectsWithTag("Agent");
		adjacentAgents = new Pair<double,double>[3];
		radius = 4;
		proj = (GameObject.FindGameObjectsWithTag("Projector"))[0];

		for(int i = 0; i < adjacentAgents.Length; ++i){
			adjacentAgents[i] = new Pair<double,double>(-1,0);
		}
		moveSpeed = .15f;
		rotateSpeed = 3f;
		inControl = false;
		rangeDistance = 4.5f;
		upDistance = rangeDistance;
		leftDistance = rangeDistance;
		rightDistance = rangeDistance;
		UpdatePosition();
	}

	// Update is called once per frame
	public void Update () {
		if(inControl){
			UpdatePosition();
			rangeFinder();
			AdjacencySensor();
			Radar();
		}
		else if(proj.transform.position == transform.position){
			proj.transform.position = new Vector3(20f,0f,0f);
		}

		//TODO Rangefinder loop, adjacency sensor loop, radar loop
	}

	private void UpdatePosition(){
		x = transform.position.x;
		y = transform.position.z;
		theta = transform.eulerAngles.y;
	}

	private void rangeFinder(){
		RaycastHit hit;
		
		Vector3 leftDir = Quaternion.AngleAxis (-30+transform.eulerAngles.y, Vector3.up) * Vector3.forward;
		Vector3 rightDir = Quaternion.AngleAxis (30+transform.eulerAngles.y, Vector3.up) * Vector3.forward;
		
		Ray upRay = new Ray(transform.position, transform.forward);
		Ray leftRay = new Ray (transform.position, leftDir);
		Ray rightRay = new Ray(transform.position, rightDir);
		
		
		Debug.DrawRay (transform.position, transform.forward*rangeDistance, Color.red);
		Debug.DrawRay (transform.position, leftDir*rangeDistance, Color.red);
		Debug.DrawRay (transform.position, rightDir*rangeDistance, Color.red);
		
		if (Physics.Raycast (upRay, out hit, rangeDistance)) {
			
			if (hit.collider.tag == "Wall") {
				upDistance = Vector3.Distance (hit.collider.transform.position, transform.position);
			}

		} else {
			upDistance = rangeDistance;
		  }

		if (Physics.Raycast (leftRay, out hit, rangeDistance)) {

			if (hit.collider.tag == "Wall") {
				leftDistance = Vector3.Distance (hit.collider.transform.position, transform.position);
			}

		} else {
			leftDistance = rangeDistance;
		  }

		if (Physics.Raycast (rightRay, out hit, rangeDistance)) {

			if (hit.collider.tag == "Wall") {
				rightDistance = Vector3.Distance (hit.collider.transform.position, transform.position);
			}
		} else {
			rightDistance = rangeDistance;
		  }

	}

	private void AdjacencySensor(){
		//TODO draw line render circle
		DrawAdjacencySensor();
		for(int i = 0; i < agents.Length; ++i){
			Transform t = agents[i].transform;
			if(t != transform){
				//check distance
				double dist = Vector3.Distance(transform.position,t.position);
				if(dist < radius){
					Vector3 targetDir = t.position - transform.position;
					Vector3 forward = transform.forward;
					double ang = Vector3.Angle(targetDir,forward);
					adjacentAgents[i] = new Pair<double,double>(ang,dist);
				}
				else {
					adjacentAgents[i] = new Pair<double,double>(-1,0);
				}
			}
			else {
				adjacentAgents[i] = new Pair<double,double>(-1,0);	
			}
		}
	}

	private void DrawAdjacencySensor(){
		proj.transform.position = transform.position;
	}
	
	private void Radar(){

	}

	private void Turn(bool dir){

		if (dir) {
			transform.Rotate(Vector3.up*rotateSpeed);
		} else {
			transform.Rotate(Vector3.down*rotateSpeed);
		}
	}

	private void Move(bool dir) {

		if(dir){
			transform.Translate(-Vector3.forward*moveSpeed);
		}
		else{
			transform.Translate(Vector3.forward*moveSpeed);
		}
	}

	private void SwitchControl(){
		inControl = !inControl;
	}

	private void ToString(StringWrapper wrap){
		StringBuilder builder = new StringBuilder();
		builder.Append("\nX: " + x +"\nY: " + y
			        +"\nHeading: " + theta
			        +"\nRangeFinder: " + PrintDistances()
		            +"\nAdjacency Sensor: "+PrintAdjacencyInfo()
					+"\nRadar: "+PrintRadarInfo());
		wrap.output = builder.ToString();
	}

	private string  PrintDistances(){
		StringBuilder builder = new StringBuilder ();
		builder.Append("\nForward distance " + upDistance + "\nLeft distance " + leftDistance + "\nRight distance " + rightDistance);

		return builder.ToString();
	}

	private string PrintAdjacencyInfo(){
		StringBuilder builder = new StringBuilder();

		builder.Append("\nAdjacency Sensor:");
		if(adjacentAgents != null){
			for(int i = 0; i < adjacentAgents.Length; ++i){
				double ang = adjacentAgents[i].First;
				double dist = adjacentAgents[i].Second;
				if(ang >= 0){
					builder.Append("\nAgent "+i
					               +":\n\t Relative Heading: "+ang
					               +"\n\tDistance: "+dist);
				}
			}
		}
		return builder.ToString();
	}

	private string PrintRadarInfo(){
		return "\nTODO";
		/*		Front: 1
			 * 		Back: 0
			 * 		Left: 1
			 * 		Right: 0
				*/
	}
}

