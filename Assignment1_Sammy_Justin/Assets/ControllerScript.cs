using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour {
	
	public KeyCode up = KeyCode.UpArrow;
	public KeyCode down = KeyCode.DownArrow;
	public KeyCode left = KeyCode.LeftArrow;
	public KeyCode right = KeyCode.RightArrow;

	public int agentNum = 0;

	public GameObject currentAgent;

	public GameObject[] agents; 

	public bool selectMode = true;

	public bool agentFlag = true;

	// Use this for initialization
	void Start () {
		agents = GameObject.FindGameObjectsWithTag("Agent");
	}
	
	// Update is called once per frame
	void Update () {
		if(agentFlag){
			bool leftBool = Input.GetKey(left);
			bool rightBool = Input.GetKey(right);

			if(leftBool){
				agentFlag = false;
				StartCoroutine(AgentSelectCool(false));
			}
			else if(rightBool){
				agentFlag = false;
				StartCoroutine(AgentSelectCool(true));
			}
		}
	}

	// Use this for initializing GUI
	void OnGUI () {
		GUI.Box(new Rect(10,10,200,50),
		        "Move arrows left and right to\n" +
		        "selet an agent, then press enter\n" +
		        "to control: Agent #" + agentNum);
	}

	IEnumerator AgentSelectCool(bool dir){
		while(!agentFlag){
			yield return new WaitForSeconds(1f/6);
			// dir == true for right, false for left
			if(dir){
				if(agentNum == agents.Length -1){
					agentNum = 0;
				}
				else{
					++agentNum;
				}
			}
			else{
				if(agentNum == 0){
					agentNum = agents.Length -1;
				}
				else{
					--agentNum;
				}
			}
			agentFlag = true;
		}
	}
}
