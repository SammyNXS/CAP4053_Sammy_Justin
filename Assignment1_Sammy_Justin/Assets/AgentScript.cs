using UnityEngine;
using System.Collections;

public class AgentScript : MonoBehaviour {
	public double x;
	public double y;
	public double theta;

	// Use this for initialization
	public void Start () {
		UpdatePosition();
	}

	// Update is called once per frame
	public void Update () {
		UpdatePosition();
		Move();
	}

	private void UpdatePosition(){
		x = transform.position.x;
		y = transform.position.y;
		theta = transform.eulerAngles.y;
	}

	private void Move(){
//		int xAxis = ((Input.GetKey(right))?1:0)-((Input.GetKey(left))?1:0);
//		xMove = xAxis*moveSpeed*Time.deltaTime;
//		
//		if((xMove != 0) && walkFlag){
//			int multfactor = 1;
//			int addFactor = 0;
//			if(xMove < 0){
//				multfactor = -1;
//				addFactor = 1;
//			}
//			if(walk == 0){
//				Debug.Log("stand");
//				setOffset((standIndex+addFactor)*multfactor);
//				++walk;
//			}
//			else if(walk == 1){
//				Debug.Log("walk1");
//				setOffset((standIndex+addFactor+1)*multfactor);
//				++walk;
//			}
//			else if(walk == 2){
//				Debug.Log("walk2");
//				setOffset((standIndex+addFactor+2)*multfactor);
//				++walk;
//			}
//			else if(walk == 3){
//				Debug.Log("back21");				
//				setOffset((standIndex+addFactor+1)*multfactor);
//				walk = 0;
//			}
//			
//			renderer.material.SetTextureOffset("_MainTex", offset);
//			walkFlag=false;
//			StartCoroutine("SpriteCool");
//		}
//		transform.
//			transform.position+= new Vector3(xMove,0,0);
	}
}
