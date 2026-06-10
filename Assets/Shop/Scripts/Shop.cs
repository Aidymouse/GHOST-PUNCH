using UnityEngine;

public class Shop : MonoBehaviour
{
  public float turnSpeed;
  public Quaternion turnGoal;

  public Transform camera_pos;
  public Transform camera_target;

	Item[] item_instances;

	ItemRecord bought;

  void Start()
  {

		bought = new ItemRecord();

    camera_target.position = camera_pos.position + Vector3.forward * 10;

		item_instances = new Item[10];
		item_instances[(int)ItemType.SALT_SHAKER] = new SaltShaker();
		item_instances[(int)ItemType.PAINKILLERS] = new Painkillers();
		item_instances[(int)ItemType.GHOST_DETECTOR] = new GhostDetector();
		item_instances[(int)ItemType.PROTEIN_POWDER] = new ProteinPowder();

  }

  void Update() { }

  public void LookTowardsShop() { }

  public void LookTowardsDoor() { }

	/* Item Management */
	public void BuyItem(ShopItem item) {
		Item instance = item_instances[(int)item.item_id];

		Debug.Log(item.item_id + " costs " + item.cost + " ectoplasm");

		bought.AddItem(instance);

	}

}
