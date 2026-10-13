using System.Collections.Generic;
using UnityEngine;

public class RandUtils {
	public static int GetIndexFromWeights(List<int> weights) {
		int total = 0;
		foreach (int weight in weights) {
			total += weight;
		}

		int roll = Random.Range(0, total);
		int running_total = 0;
		for (int i=0; i < weights.Count; i++) {
			if (running_total <= roll && roll < running_total + weights[i]) {
				return i;
			}

			running_total += weights[i];
		}

		return -1;

	}
}
