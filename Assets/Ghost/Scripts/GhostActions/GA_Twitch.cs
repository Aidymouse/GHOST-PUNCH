using UnityEngine;

public class GA_Twitch : GhostAction {
	Timer ti_twitch;
	GAD_Twitch twitch_data;

	public GA_Twitch(Ghost g) : base(g) {
		twitch_data = ghost.defaults.twitch_data;
		ti_twitch = new Timer(0);
	}

	public override void Enter() {
		int twitch_time = Random.Range(twitch_data.MIN_DURATION, twitch_data.MAX_DURATION+1);
		Debug.Log("Twitch time set to "+twitch_time);
		ti_twitch.SetTime(twitch_time, twitch_time);
	}

	public override void Update() {
		ti_twitch.Tick(Time.deltaTime);
		if (ti_twitch.Finished()) {
			ghost.ExitAction();
		}
	}


}
