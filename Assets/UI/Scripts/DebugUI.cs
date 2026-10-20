using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{

  public Ghost ghost;
  public GhostPuncher ghost_puncher;

  public TMP_Text ghost_action;
  public TMP_Text ghost_action_state;

  public TMP_Text gp_statuses;

	public Aberrator aberrator;
	public TMP_Text aberration_state;
	
  void Start() { }

  void Update() {
		
		UpdateGhostActionPanel();
		UpdateGPStatusPanel();

		UpdateAberrationPanel();

  }

	void UpdateGhostActionPanel() {
		ghost_action.SetText("" + ghost.cur_action);
		ghost_action_state.SetText(ghost.GetCurrentAction()?.GetDebugString() ?? "");
	}

	void UpdateGPStatusPanel() {
		string s = "";
		foreach (StatusEffect status in ghost_puncher.statuses) {
			s += status.GetDebugString() + "\n";
		}
		gp_statuses.SetText(s);
	}

	
	void UpdateAberrationPanel() {
		aberration_state.SetText(aberrator.GetDebugString());
	}

}
