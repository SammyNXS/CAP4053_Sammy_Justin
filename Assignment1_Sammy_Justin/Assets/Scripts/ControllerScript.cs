using UnityEngine;
using System.Collections;

public class ControllerScript: MonoBehaviour {
	
	public KeyCode up = KeyCode.UpArrow;
	public KeyCode down = KeyCode.DownArrow;
	public KeyCode left = KeyCode.LeftArrow;
	public KeyCode right = KeyCode.RightArrow;

	public int agentNum = 0;

	public GameObject currentAgent;

	public GameObject[] agents; 

	public bool selectMode = true;

	public bool agentFlag = true;

	public bool modeFlag = true;

	// Use this for initialization
	void Start () {
		agents = GameObject.FindGameObjectsWithTag("Agent");
		currentAgent = agents [0];
	}
	
	// Update is called once per frame
	void Update () {
		bool leftBool = Input.GetKey(left);
		bool rightBool = Input.GetKey(right);
		bool upBool = Input.GetKey(up);
		bool downBool = Input.GetKey(down);

		bool spaceBool =Input.GetKey(KeyCode.Space);

		if(selectMode && agentFlag){
			if(spaceBool && modeFlag){
				currentAgent = agents[agentNum];
				modeFlag = false;
				selectMode = false;
				currentAgent.SendMessage("SwitchControl");
				StartCoroutine("ModeSelectCool");
			}
			if(leftBool || upBool){
				agentFlag = false;
				StartCoroutine(AgentSelectCool(false));
			}
			else if(rightBool || downBool){
				agentFlag = false;
				StartCoroutine(AgentSelectCool(true));
			}
		}
		else if (!selectMode){
			if(spaceBool && modeFlag){
				//currentAgent = agents[agentNum];
				selectMode = true;
				modeFlag = false;
				currentAgent.SendMessage("SwitchControl");
				StartCoroutine("ModeSelectCool");
			}
			if(leftBool){
				currentAgent.SendMessage("Turn",false);
			}
			else if(rightBool){
				currentAgent.SendMessage("Turn",true);
			}
			else if(upBool){
				currentAgent.SendMessage("Move",false);
			}
			else if(downBool){
				currentAgent.SendMessage("Move",true);
			}
		}
	}

	// Use this for initializing GUI
	void OnGUI () {
		if (selectMode) {
			GUI.Box(new Rect(10,10,225,60),
			        "Move arrows left and right to\n" +
			        "selet an agent, then press spacebar\n" +
			        "to control: Agent #" + agentNum);
		}
		else{
			GUI.Box(new Rect(Screen.width-235,10,225,300),
			        "Current Agent: " + agentNum 
			        +  getCurrentAgentData());
		}
	}

	private string getCurrentAgentData(){
		StringWrapper wrap = new StringWrapper();
		currentAgent.SendMessage ("ToString", wrap);
		return wrap.output;
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

	IEnumerator ModeSelectCool(){
		while (!modeFlag) {
			yield return new WaitForSeconds(1f/6);
			modeFlag = true;
		}
	}
}

public class StringWrapper {
	public string output = "blah";
	public StringWrapper() {
	}
}
