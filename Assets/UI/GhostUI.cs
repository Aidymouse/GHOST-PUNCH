using UnityEngine;
using TMPro;

public class GhostUI : MonoBehaviour
{
		public Ghost ghost;
		public GhostPuncher ghost_puncher;

    TMP_Text ui_escape_meter;
    TMP_Text ui_ectoplasm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();

      foreach (TMP_Text text in texts) {
	switch (text.name) {
	  case "EscapeMeter": {
	    ui_escape_meter = text;
	    break;
	  }
	  case "Ectoplasm": {
	    ui_ectoplasm = text;
	    break;
	  }
	}
      }

      //ui_escape_meter = UnityEngine.GameObject.Find<TMP_Text>("EscapeMeter");
    }

    // Update is called once per frame
    void Update()
    {
			UpdateEscapeMeter(ghost.escape_meter);
        
    }

    public void UpdateEscapeMeter(float value) {

      ui_escape_meter.SetText("" + value);

    }

    public void UpdateEctoplasm(int plasm) {
      ui_ectoplasm.SetText("Ectoplasm: " + plasm);
    }
}
