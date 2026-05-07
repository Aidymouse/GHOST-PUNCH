using UnityEngine;
using UnityEngine.EventSystems;

public interface ShopUIEventHandler : IEventSystemHandler {
	void ClickRight();
	void ClickLeft();
}

public class ShopUI : MonoBehaviour, ShopUIEventHandler
{

		public ShopCamera camera;

    void Start()
    {
    }

    void Update()
    {
        
    }

		public void ClickRight() {
			Debug.Log("Clicking Right");
			camera.turnGoal *= Quaternion.Euler(0, 90, 0);
		}

		public void ClickLeft() {
			camera.turnGoal *= Quaternion.Euler(0, -90, 0);
		}
}
