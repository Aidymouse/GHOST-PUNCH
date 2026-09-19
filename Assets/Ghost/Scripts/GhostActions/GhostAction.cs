public class GhostAction {

	public Ghost ghost;

	public GhostAction(Ghost g) { ghost = g; }

	public virtual void Enter() { }
	public virtual void Exit() { }

	public virtual void Pause() { }
	public virtual void Resume() { }

	public virtual void Update() { }

}
