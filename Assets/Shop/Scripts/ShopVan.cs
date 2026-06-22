using UnityEngine;

public class ShopVan : MonoBehaviour {

	public Animator van_animator;

	public void Awake() {}
	public void Start() {
		ChangeAnimation("VanDoorOpenAnim");
	}

	public void Update() {}

	public void ChangeAnimation(string name, float fade=0) {
		van_animator.CrossFade(name, fade);
	}
	
	 
}
