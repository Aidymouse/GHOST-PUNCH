using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GhostDefaults", menuName = "Scriptable Objects/Ghost/GhostActionSet")]
public class GhostActionSet : ScriptableObject {
	[Tooltip("List of actions taken at this range")]
	public List<GhostActions> actions;
	[Tooltip("Weights for actions. Weight[N] for action[N]")]
	public List<int> weights;
	public bool debug;
	public GhostActions debug_action;

	public GhostActions WeightedPickAction() {
		int idx = RandUtils.GetIndexFromWeights(this.weights);
		if (idx == -1) {
			return actions[actions.Count];
		}
		return actions[idx];
	}
}
