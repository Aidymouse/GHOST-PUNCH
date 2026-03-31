using UnityEngine;

class GhostPower {
  public Ghost ghost;

  protected Timer ti_charge;
  protected Timer ti_active_delay;
  protected Timer ti_active;
  protected Timer ti_hang;

  enum GhostPowerPhase {
    PRE_CHARGE,
    CHARGING,
    ACTIVE_DELAY,
    ACTIVE,
    HANG,
    POST_HANG,
  };

  GhostPowerPhase phase;

  public GhostPower(Ghost myghost, float charge_timer, float active_delay_timer, float active_timer, float hang_timer) {
    this.ghost = myghost;
    this.ti_charge = new Timer(0);
    this.ti_active_delay = new Timer(0);
    this.ti_active = new Timer(0);
    this.ti_hang = new Timer(0);

    this.ti_charge.deactive();
    this.ti_active_delay.deactive();
    this.ti_active.deactive();
    this.ti_hang.deactive();

    this.phase = GhostPowerPhase.PRE_CHARGE;

  }

  public void Start() {
    ti_charge.activate();
    StartCharge();
    phase = GhostPowerPhase.CHARGING;
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
    ti_charge.tick(Time.deltaTime);
    ti_active_delay.tick(Time.deltaTime);
    ti_active.tick(Time.deltaTime);
    ti_hang.tick(Time.deltaTime);
  }

  public void HandleEvents() {

    if (ti_charge.finished()) {
      OnEndCharge();
      ti_charge.deactivate();

      ti_active_delay.activate();
      OnStartActiveDelay();
      phase = GhostPowerPhase.ACTIVE_DELAY;
    } else if (ti_charge.is_active()) {
      OnUpdateCharge();
    }

    if (ti_active_delay.finished()) {
      OnEndActiveDelay();
      ti_active_delay.deactivate();

      ti_active.activate();
      OnStartActive();
      phase = GhostPowerPhase.ACTIVE;
    } else if (ti_active_dealy.is_active()) {
      OnUpdateActiveDelay();
    }

    if (ti_active.finished()) {
      OnEndActive();
      ti_active.deactivate();

      ti_hang.activate();
      OnStartHang();
      phase = GhostPowerPhase.HANG;
    } else if (ti_active.is_active()) {
      OnUpdateActive();
    }

    if (ti_hang.finished()) {
      OnEndHang();
      ti_hang.deactivate();
      phase = GhostPowerPhase.POST_HANG;

    } else if (ti_hang.is_active()) {
      OnUpdateHang();
    }

  }

  public void End() {}

  public void OnStartCharge() {}
  public void OnUpdateCharge() {}
  public void OnEndCharge() {}

  public void OnStartActiveDelay() {}
  public void OnUpdateActiveDelay() {}
  public void OnEndActiveDelay() {}

  public void OnStartActive() {}
  public void OnUpdateActive() {}
  public void OnEndActive() {}

  public void OnStartHang() {}
  public void OnUpdateHang() {}
  public void OnEndHang() {}


}



