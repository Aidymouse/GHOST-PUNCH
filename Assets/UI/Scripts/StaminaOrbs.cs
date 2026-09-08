using UnityEngine;

public class StaminaOrbs : MonoBehaviour
{
		public float stamina_per_orb;
		public StaminaOrb[] orbs;

		float s;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
		void Awake() {
			orbs = GetComponentsInChildren<StaminaOrb>();
			s = 150.0f;
		}


    void Start()
    {
		
        
    }

    // Update is called once per frame
    void Update()
    {
			s -= Time.deltaTime*2;
			SetStamina(s);
        
    }

		public void SetStamina(float stamina) {
			float remainder = stamina % stamina_per_orb;
			float orb_idx_f = (stamina - remainder) / stamina_per_orb;
			int orb_idx = (int)orb_idx_f;
			Debug.Log("Accessing stamina orb at idx "+orb_idx);
			if (orb_idx == -1) { return; }

			orbs[orb_idx].SetPortion(remainder / stamina_per_orb);
		
		}
}
