using UnityEngine;

class Lerp {
	public static float lerp(float start, float end, float pct) {
		return (start + (end - start) * pct);
	}

	public static float EaseInCubic(float t) { return t * t * t; }

	public static float EaseOutCubic(float t) {
		return 1f - (1f - t) * (1f - t) * (1f - t);
	}
	public static float EaseOutExpo(float t, float pow) {
    return t == 1f ? 1f : 1f - Mathf.Pow(pow, -10f * t);
	}
	/* getting kind of close, but not really */
	/*
	public static float LerpIWant(float t) {
		if (t == 0f) { return 0f; }
		return 1+(Mathf.Log(t)/2);
	}
	*/
}

