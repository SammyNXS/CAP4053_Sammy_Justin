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
		return "\nTODO";
		/*
		 * 
		 * Distance 0: #
		 * Distance 1: #
		 * Distance 2: #
		 */
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

