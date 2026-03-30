using UnityEngine;

class GhostPower {
  public Ghost ghost;

  protected Timer ti_charge;
  protected Timer ti_active_delay;
  protected Timer ti_active;
  protected Timer ti_hang;

  enum GhostPowerPhase {
    CHARGING,
    ACTIVE_DELAY,
    ACTIVE,
    HANG,
  };

  GhostPowerPhase phase;

  public GhostPower(Ghost myghost) {
    this.ghost = myghost;
    this.ti_charge = new Timer(0);
    this.ti_active_delay = new Timer(0);
    this.ti_active = new Timer(0);
    this.ti_hang = new Timer(0);

    this.ti_charge.deactive();
    this.ti_active_delay.deactive();
    this.ti_active.deactive();
    this.ti_hang.deactive();

    this.phase = GhostPowerPhase.CHARGING;

  }

  public void Start() {
  }

  /** Default update fn just does these events
   * More hands on updates can implement their own update method
   * **/
  public void Update() {
    // Call relevant methods and update timers
    UpdateTimers();
    HandleEvents();
  }

  public void UpdateTimers() {
    // Tick timers
  }

  public void HandleEvents() {
  }

  public void End() {
  }

  public void StartCharge() {
  }

  public void UpdateCharge() {
  }

  public void EndCharge() {
  }

  public void StartActiveDelay() {
  }

  public void UpdateActiveDelay() {
  }

  public void EndActiveDelay() {
  }



}



