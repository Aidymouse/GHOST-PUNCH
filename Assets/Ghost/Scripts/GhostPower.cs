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

    this.ti_charge.Deactivate();
    this.ti_active_delay.Deactivate();
    this.ti_active.Deactivate();
    this.ti_hang.Deactivate();

    this.phase = GhostPowerPhase.PRE_CHARGE;

  }

  public virtual void Start() {
    ti_charge.Activate();
    OnStartCharge();
    phase = GhostPowerPhase.CHARGING;
  }

	/* Power objects get instantiated once and re-used. The reset method puts a power back into its first state */
  public virtual void Reset() {
		ti_charge.Deactivate();
		ti_active_delay.Deactivate();
		ti_active.Deactivate();
		ti_hang.Deactivate();
		ti_charge.Reset();
		ti_active_delay.Reset();
		ti_active.Reset();
		ti_hang.Reset();
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
    ti_charge.Tick(Time.deltaTime);
    ti_active_delay.Tick(Time.deltaTime);
    ti_active.Tick(Time.deltaTime);
    ti_hang.Tick(Time.deltaTime);
  }

  public void HandleEvents() {

    if (ti_charge.Finished()) {
      OnEndCharge();
      ti_charge.Deactivate();

      ti_active_delay.Activate();
      OnStartActiveDelay();
      phase = GhostPowerPhase.ACTIVE_DELAY;
    } else if (ti_charge.IsActive()) {
      OnUpdateCharge();
    }

    if (ti_active_delay.Finished()) {
      OnEndActiveDelay();
      ti_active_delay.Deactivate();

      ti_active.Activate();
      OnStartActive();
      phase = GhostPowerPhase.ACTIVE;
    } else if (ti_active_delay.IsActive()) {
      OnUpdateActiveDelay();
    }

    if (ti_active.Finished()) {
      OnEndActive();
      ti_active.Deactivate();

      ti_hang.Activate();
      OnStartHang();
      phase = GhostPowerPhase.HANG;
    } else if (ti_active.IsActive()) {
      OnUpdateActive();
    }

    if (ti_hang.Finished()) {
      phase = GhostPowerPhase.POST_HANG;

      OnEndHang();
      ti_hang.Deactivate();

    } else if (ti_hang.IsActive()) {
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



