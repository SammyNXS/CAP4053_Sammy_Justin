using UnityEngine;
using System.Collections;

// Script that acts as a high-level controller for GUIs and controlling an agent
public class ControllerScript: MonoBehaviour {

	// Key code variables for menu and movement
	public static KeyCode up = KeyCode.UpArrow;
	public static KeyCode down = KeyCode.DownArrow;
	public static KeyCode left = KeyCode.LeftArrow;
	public static KeyCode right = KeyCode.RightArrow;
	public static KeyCode space = KeyCode.Space;

	// Current Agent data
	public int agentNum;
	public GameObject currentAgent;

	// All agents in environment
	public GameObject[] agents; 

	// If true, in select menu. Else, in control mode
	public bool selectMode;

	// Flags for cooldown subroutines
	public bool agentFlag;
	public bool modeFlag;

	// Use this for initialization
	void Start () {
		// Populate list of agents and set a default value
		agents = GameObject.FindGameObjectsWithTag("Agent");
		agentNum = 0;
		currentAgent = agents [0];
		selectMode = true;
		agentFlag = true;
		modeFlag = true;
	}
	
	// Update is called once per frame
	void Update () {
		// Each of these is true if key pressed, else false;
		bool leftBool = Input.GetKey(left);
		bool rightBool = Input.GetKey(right);
		bool upBool = Input.GetKey(up);
		bool downBool = Input.GetKey(down);
		bool spaceBool =Input.GetKey(space);

		// If in select mode and can select agent
		if(selectMode && agentFlag){
			// Change mode to control
			if(spaceBool && modeFlag){
				currentAgent = agents[agentNum];
				modeFlag = false;
				selectMode = false;
				currentAgent.SendMessage("SwitchControl");
				StartCoroutine("ModeSelectCool");
			}
			// Move cursor left
			if(leftBool || upBool){
				agentFlag = false;
				StartCoroutine(AgentSelectCool(false));
			}
			// Move cursor right
			else if(rightBool || downBool){
				agentFlag = false;
				StartCoroutine(AgentSelectCool(true));
			}
		}
		// If in control mode
		else if (!selectMode){
			// Change mode to selection
			if(spaceBool && modeFlag){
				selectMode = true;
				modeFlag = false;
				currentAgent.SendMessage("SwitchControl");
				StartCoroutine("ModeSelectCool");
			}
			// Turn left or right
			if(leftBool || rightBool){
				currentAgent.SendMessage("Turn",rightBool);
			}
			// Move forward or back
			else if(upBool || downBool){
				currentAgent.SendMessage("Move",downBool);
			}
		}
	}

	// Use this for initializing GUI
	void OnGUI () {
		// GUI Display for selection mode
		if (selectMode) {
			GUI.Box(new Rect(10,10,225,90),
			        "Press left and right to selet\n" +
			        "an agent, then press spacebar\n" +
			        "to control that agent.\n\n" +
			        "Agent #" + agentNum);
		}
		// GUI Display for control mode
		else{
			GUI.Box(new Rect(Screen.width-235,10,225,600),
			        "Press up and down to move forward\n" +
			        "and backward, respectively, relative\n" +
			        "to current heading. Press left and\n" +
			        "right to turn heading. Press spacebar\n" +
			        "to return to select menu." +
			        "\n\nCurrent Agent: " + agentNum 
			        +  getCurrentAgentData());
		}
	}

	// Call ToString on the current agent
	private string getCurrentAgentData(){
		StringWrapper wrap = new StringWrapper();
		currentAgent.SendMessage ("ToString", wrap);
		return wrap.output;
	}

	// Cooldown for selecting an agent in the selection menu (prevents hectic
	// rapid scrolling in selection mode). If dir is true, pushing cursor
	// right. Else, left.
	IEnumerator AgentSelectCool(bool dir){
		while(!agentFlag){
			yield return new WaitForSeconds(1f/6);
			int checkNum = dir? (agents.Length-1):0;
			if(agentNum == checkNum){
				agentNum = dir ? 0 : (agents.Length-1);
			}
			else{
				agentNum += dir ? 1:-1;
			}
			agentFlag = true;
		}
	}

	// Cooldown for selecting the current mode (prevents hectic rapid changing of mode)
	IEnumerator ModeSelectCool(){
		while (!modeFlag) {
			yield return new WaitForSeconds(1f/6);
			modeFlag = true;
		}
	}
}
