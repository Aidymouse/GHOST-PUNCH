using UnityEngine;
using UnityEngine.EventSystems;

public interface ShopUIEventHandler : IEventSystemHandler {
	void ClickRight();
}

public class ShopUI : MonoBehaviour, ShopUIEventHandler
{
    void Start()
    {
    }

    void Update()
    {
        
    }

		public void ClickRight() {
			Debug.Log("Hello");
		}
}
