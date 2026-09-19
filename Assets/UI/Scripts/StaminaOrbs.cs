using UnityEngine;

public class StaminaOrbs : MonoBehaviour
{
		public float stamina_per_orb;
		public StaminaOrb[] orbs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
		void Awake() {
			orbs = GetComponentsInChildren<StaminaOrb>();
		}


    void Start()
    {
		
        
    }

    // Update is called once per frame
    void Update() { }

		public void SetStamina(float stamina) {
			//Debug.Log("Accessing stamina orb at idx "+orb_idx);

			for (int i=0; i<orbs.Length; i++) {
				float orb_max = stamina_per_orb * (i+1);
				if (orb_max <= stamina) {
					orbs[i].SetPortion(1);
				} else {
					float dist = orb_max - stamina;
					if (dist < stamina_per_orb) {
						orbs[i].SetPortion((stamina_per_orb - dist) / stamina_per_orb);
					} else {
						orbs[i].SetPortion(0);
					}
				}
	
			}

		
		}
}
