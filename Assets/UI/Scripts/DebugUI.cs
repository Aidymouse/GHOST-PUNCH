using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
  string[] ghost_action_strings;

  public Ghost ghost;
  public GhostPuncher ghost_puncher;

  TMP_Text ui_debug1; // Ghost action
  TMP_Text ui_debug2; // Ghost hitstun timer
  TMP_Text ui_debug3; 

  TMP_Text ghost_state;
  TMP_Text ghost_power;
  TMP_Text ghost_power_timer;

  void Start()
  {

		ghost_action_strings = new string[20];
		ghost_action_strings[(int)GhostActions.MOVING_ROOM] = "Moving Room";
		ghost_action_strings[(int)GhostActions.STARTLED] = "Startled";
		ghost_action_strings[(int)GhostActions.CHARGING_ESCAPE] = "Charging Escape";

    TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();

    foreach (TMP_Text text in texts) {
      switch (text.name) {
				case "Debug1": 
					ui_debug1 = text;
					break;

				case "Debug2": 
					ui_debug2 = text;
					break;

				case "Debug3": 
					ui_debug3 = text;
					break;


				case "DebugGhostState": 
					ghost_state = text;
					break;

				case "DebugGhostPower": 
					ghost_power = text;
					break;

				case "DebugGhostPowerTimer": 
					ghost_power_timer = text;
					break;

      }
    }
  }

  void Update() {
    SetDebug1("Ghost Action: "+ghost_action_strings[(int)ghost.cur_action]);
    SetDebug2("Ragdoll timer: "+ghost.ti_ragdoll.time_remaining);

  }

  public void SetDebug1(string text){
    ui_debug1.SetText(text);
  }

  public void SetDebug2(string text){
    ui_debug2.SetText(text);
  }

  public void SetDebug3(string text){
    ui_debug3.SetText(text);
  }

  public void SetGhostState(Ghost ghost, string state, string power, string power_timer) {
    ghost_state.SetText(state);
    ghost_power.SetText(power);
    ghost_power_timer.SetText(power_timer);
  }
}
