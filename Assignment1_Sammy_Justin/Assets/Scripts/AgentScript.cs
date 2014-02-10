using UnityEngine;
using System.Collections;
using System.Text;

// Script for altering and tracking positon and heading of agent, as well as 
// running a set of sensors
public class AgentScript : MonoBehaviour {

	// Coordinates and heading of agent
	public float x;
	public float y;
	public float theta;

	// Defined speeds for moving and rotation
	public float moveSpeed;
	public float rotateSpeed;

	// Range used for all sensors
	public float range;

	// Current activation levels for radar slices
	public int frontLevel;
	public int backLevel;
	public int leftLevel;
	public int rightLevel;

	// Boolean for judging if this agent is currently user controlled
	public bool inControl;

	// Object to access projector, which creates approximation of
	// adjacency sensor range
	public GameObject proj;

	// List of current agents and adjacent agent info
	public GameObject[] agents;
	public Pair<double,double>[] adjacentAgents;

	// Current distances for each arm of rangefinder
	public double upDistance;
	public double leftDistance;
	public double rightDistance;

	// Vectors used for both calcuation and drawing of radar
	public Vector3 forwLeft;
	public Vector3 forwRight;
	public Vector3 backLeft;
	public Vector3 backRight;
	public Vector3 endBL;
	public Vector3 endBR;
	public Vector3 endFL;
	public Vector3 endFR;


	// Use this for initialization
	public void Start () {
		// Populate agents and adjacent agents lists, the latter
		// with "null" values
		agents = GameObject.FindGameObjectsWithTag("Agent");
		adjacentAgents = new Pair<double,double>[agents.Length];
		for(int i = 0; i < adjacentAgents.Length; ++i){
			adjacentAgents[i] = new Pair<double,double>(-1,0);
		}
		// Get projector object from environment
		proj = (GameObject.FindGameObjectsWithTag("Projector"))[0];
		moveSpeed = .15f;
		rotateSpeed = 3f;
		inControl = false;
		range = 4.5f;
		upDistance = range;
		leftDistance = range;
		rightDistance = range;
		initSlices();
		UpdatePosition();
	}
	
	// Update is called once per frame
	public void Update () {
		// If in control, run sensors
		if(inControl){
			UpdatePosition();
			RangeFinder();
			AdjacencySensor();
			Radar();
		}
		// Else, move projector for Ad
		else if(proj.transform.position == transform.position){
			proj.transform.position = new Vector3(20f,0f,0f);
		}
	}

	// Initialize activation radar slices
	private void initSlices(){
		frontLevel = 0;
		backLevel = 0;
		leftLevel = 0;
		rightLevel = 0;
	}

	// Update postion and heading data for ease of calculations and printing
	private void UpdatePosition(){
		x = transform.position.x;
		y = transform.position.z;
		theta = transform.eulerAngles.y;
	}

	// Calculate range finder distances and draw range finder rays
	private void RangeFinder(){
		Vector3 leftDir = Quaternion.AngleAxis (-30+transform.eulerAngles.y, Vector3.up) * Vector3.forward;
		Vector3 rightDir = Quaternion.AngleAxis (30+transform.eulerAngles.y, Vector3.up) * Vector3.forward;
		
		Ray upRay = new Ray(transform.position, transform.forward);
		Ray leftRay = new Ray (transform.position, leftDir);
		Ray rightRay = new Ray(transform.position, rightDir);

		// Draws the rays (in debug environment)
		Debug.DrawRay (transform.position, transform.forward*range, Color.red);
		Debug.DrawRay (transform.position, leftDir*range, Color.red);
		Debug.DrawRay (transform.position, rightDir*range, Color.red);

		upDistance = CalcDistance(upRay);
		leftDistance = CalcDistance (leftRay);
		rightDistance = CalcDistance (rightRay);
	}

	// Calculate distance based on range
	private double CalcDistance(Ray ray){
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, range) 
		    && hit.collider.tag == "Wall") {
				return Vector3.Distance (hit.point, transform.position);
		}else {
			return range;
		}
	}

	// Adjacency sensor function for drawing & calculating
	private void AdjacencySensor(){
		DrawAdjacencySensor();
		CalcAdjacency();
	}

	// Move projector to position of agent, which approximates visually the
	// bounds of the adjacency sensor
	private void DrawAdjacencySensor(){
		proj.transform.position = transform.position;
	}

	// Calculate which agents are in the adjacency sensor's bounds
	private void CalcAdjacency(){
		// For each agent (not including the current agent)
		for(int i = 0; i < agents.Length; ++i){
			Transform t = agents[i].transform;
			if(t != transform){
				// Check if distance is less than the range
				double dist = Vector3.Distance(transform.position,t.position);
				if(dist < range){
					Vector3 targetDir = t.position - transform.position;
					Vector3 forward = transform.forward;
					double ang = Vector3.Angle(targetDir,forward);
					// Set adjacent agent info with position and relative
					//heading angle
					adjacentAgents[i] = new Pair<double,double>(ang,dist);
				}
				// If not in sensor, set to a defined "null" value, (-1.0,0.0)
				else {
					adjacentAgents[i] = new Pair<double,double>(-1.0,0.0);
				}
			}
			else {
				adjacentAgents[i] = new Pair<double,double>(-1.0,0.0);	
			}
		}
	}
	// Calculate radar slice vectors, draw radar, and calculate activation levels 
	private void Radar(){
		forwLeft = Quaternion.AngleAxis (-45+theta, Vector3.up) * Vector3.forward;
		forwRight = Quaternion.AngleAxis (45+theta, Vector3.up) * Vector3.forward;
		backLeft = Quaternion.AngleAxis (-45+theta, Vector3.up) * -Vector3.forward;
		backRight = Quaternion.AngleAxis (45+theta, Vector3.up) * -Vector3.forward;
		DrawRadar();
		CalcRadar();
	}

	// Draw slices of radar
	private void DrawRadar(){
		Vector3 start = transform.position;
		endFL = start + (forwLeft.normalized *range);
		endFR = start + (forwRight.normalized * range);
		endBL = start + (backLeft.normalized *range);
		endBR = start + (backRight.normalized * range);
		Debug.DrawLine (endFL,endFR,Color.blue);
		Debug.DrawLine (endFR,endBL,Color.blue);
		Debug.DrawLine (endBR,endBL,Color.blue);
		Debug.DrawLine (endBR,endFL,Color.blue);
		Debug.DrawRay (start, forwLeft*range, Color.blue);
		Debug.DrawRay (start, forwRight*range, Color.blue);
		Debug.DrawRay (start, backLeft*range, Color.blue);
		Debug.DrawRay (start, backRight*range, Color.blue);
	}
	// Calculate activation levels of each slice of the radar
	private void CalcRadar(){
		initSlices();
		// For each agent (that isn't the current agent), check if they are in
		// any of the radar slices
		for(int i = 0; i < agents.Length; ++i){
			Transform t = agents[i].transform;
			if(t != transform){
				// Increment activation levels
				frontLevel+= CalcSlice(endFL,endFR,t.position);
				backLevel += CalcSlice(endBL,endBR,t.position);
				leftLevel += CalcSlice(endBR,endFL,t.position);
				rightLevel += CalcSlice(endFR,endBL,t.position);
			}
		}
	}

	// Utilizes barycentric coordinate calculations to determine if
	// the center of the agent is in the slice. Returns 1 if it is
	// in the triangle, else returns 0
	private int CalcSlice(Vector3 u, Vector3 v, Vector3 point){
		// Get points of triangle
		Vector3 p1 = v - transform.position;
		Vector3 p2 = u - transform.position;
		Vector3 p3 = point - transform.position;

		// Calculate dot products of each pair of points
		double dot00 = Vector3.Dot(p1, p1);
		double dot01 = Vector3.Dot(p1, p2);
		double dot02 = Vector3.Dot(p1, p3);
		double dot11 = Vector3.Dot(p2, p2);
		double dot12 = Vector3.Dot(p2, p3);
				
		// Determine if alpha and beta values of calculations are
		// nonzero, and alpha+beta less than 1
		double denom = (dot00 * dot11 - dot01 * dot01);
		double beta = (dot11 * dot02 - dot01 * dot12) / denom;
		double alpha = (dot00 * dot12 - dot01 * dot02) / denom;
		if((beta>= 0) && (alpha >= 0) && (beta + alpha < 1)){
			return 1;
		}
		return 0;
	}

	// Turn in direction (right if dir == true, else left)
	private void Turn(bool dir){
		transform.Rotate((dir?Vector3.up:Vector3.down)*rotateSpeed);
	}

	// Move in direction (backward if true, else forward)
	private void Move(bool dir) {
		transform.Translate((dir?-1:1)*Vector3.forward*moveSpeed);
	}

	// Sets control boolean true or false
	private void SwitchControl(){
		inControl = !inControl;
	}

	// To string function for agent. Puts string in StringWrapper object for
	// ControllerScript to access for GUI printing
	private void ToString(StringWrapper wrap){
		StringBuilder builder = new StringBuilder();
		builder.Append("\nX: " + x +"\nY: " + y
			        +"\nHeading: " + theta
			        + PrintDistances()
		            + PrintAdjacencyInfo()
					+ PrintRadarInfo());
		wrap.output = builder.ToString();
	}

	// Return string info for RangeFinder distances
	private string  PrintDistances(){
		StringBuilder builder = new StringBuilder ();
		builder.Append("\n\nRangeFinder Distances:" +
			"\nForward: " + upDistance + 
		     "\nLeft: " + leftDistance +
		     "\nRight: " + rightDistance);
		return builder.ToString();
	}

	// Return string of position and relative heading angle for each
	// agent in the adjacency sensor
	private string PrintAdjacencyInfo(){
		StringBuilder builder = new StringBuilder();
		builder.Append("\n\nAdjacency Sensor:");
		if(adjacentAgents != null){
			// For each agent, print angle and distance
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

	// Return string of activation levels for each pie slice
	private string PrintRadarInfo(){
		StringBuilder builder = new StringBuilder();
		builder.Append("\n\nRadar:\nFront: "+frontLevel
		               +"\nBack: "+backLevel
		               +"\nLeft: "+leftLevel
		               +"\nRight: "+rightLevel);
		return builder.ToString();
	}
}

