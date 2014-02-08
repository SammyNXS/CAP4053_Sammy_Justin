using UnityEngine;
using System.Collections;

public class AgentScript : MonoBehaviour {

	public double x;
	public double y;
	public double theta;
	public float moveSpeed;
	public float rotateSpeed;
	public float temp;
	
	// Use this for initialization
	public void Start () {
		moveSpeed = .25f;
		rotateSpeed = 5f;
		UpdatePosition();
		rangeFinder();
	}

	// Update is called once per frame
	public void Update () {
		UpdatePosition();
		//TODO Rangefinder loop, adjacency sensor loop, radar loop
	}

	private void UpdatePosition(){
		x = transform.position.x;
		y = transform.position.y;
		theta = transform.eulerAngles.y;
	}

	private void rangeFinder(){
	
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
}

