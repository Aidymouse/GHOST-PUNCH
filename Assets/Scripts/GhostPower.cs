using UnityEngine;

class GhostPower {
  public Ghost ghost;

  protected Timer ti_charge;
  protected Timer ti_active_delay;
  protected Timer ti_active;
  protected Timer ti_hang;

  public GhostPowerAttribs attrs;

  public enum GhostPowerPhase {
    PRE_CHARGE,
    CHARGING,
    ACTIVE_DELAY,
    ACTIVE,
    HANG,
    POST_HANG,
    /* Signals to the Ghost that this power is finished */
    DONE,
  };

  public GhostPowerPhase phase;

  public GhostPower(Ghost myghost, GhostPowerAttribs attrs, float charge_timer, float active_delay_timer, float active_timer, float hang_timer) {
    this.ghost = myghost;
    this.attrs = attrs;
    this.ti_charge = new Timer(charge_timer, charge_timer);
    this.ti_active_delay = new Timer(active_delay_timer, active_delay_timer);
    this.ti_active = new Timer(active_timer, active_timer);
    this.ti_hang = new Timer(hang_timer, hang_timer);

    this.ti_charge.deactivate();
    this.ti_active_delay.deactivate();
    this.ti_active.deactivate();
    this.ti_hang.deactivate();

    this.phase = GhostPowerPhase.PRE_CHARGE;

  }

  public virtual void Start() {
    ti_charge.activate();
    OnStartCharge();
    phase = GhostPowerPhase.CHARGING;
  }

	/* Power objects get instantiated once and re-used. The reset method puts a power back into its first state */
  public virtual void Reset() {
		ti_charge.deactivate();
		ti_active_delay.deactivate();
		ti_active.deactivate();
		ti_hang.deactivate();
		ti_charge.reset();
		ti_active_delay.reset();
		ti_active.reset();
		ti_hang.reset();
		this.phase = GhostPowerPhase.PRE_CHARGE;
  }

  /** Default update fn just does these events
   * More hands on updates can implement their own update method
   * **/
  public virtual void Update() {
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
    } else if (ti_active_delay.is_active()) {
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
      phase = GhostPowerPhase.POST_HANG;

      OnEndHang();
      ti_hang.deactivate();

    } else if (ti_hang.is_active()) {
      OnUpdateHang();
    }

  }

  public virtual void End() {}

  public virtual void OnStartCharge() {}
  public virtual void OnUpdateCharge() {}
  public virtual void OnEndCharge() {}

  public virtual void OnStartActiveDelay() {}
  public virtual void OnUpdateActiveDelay() {}
  public virtual void OnEndActiveDelay() {}

  public virtual void OnStartActive() {}
  public virtual void OnUpdateActive() {}
  public virtual void OnEndActive() {}

  public virtual void OnStartHang() {}
  public virtual void OnUpdateHang() {}

  /** By default, if we're not doing anything, we just mark ourselves as done **/
  public virtual void OnEndHang() {
    phase = GhostPowerPhase.DONE;
  }



}



