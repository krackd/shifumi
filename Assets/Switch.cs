using UnityEngine;
using UnityEngine.Events;

public abstract class Switch : MonoBehaviour
{
	public Pawn.PawnType type;
	public Player player;

	public UnityEvent OnSwitchTriggeredEvent;

	protected GameManager GameManager { get; private set; }

	private void Start()
	{
		GameManager = GameService.FindGameManager();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (player == null)
		{
			return;
		}

		Pawn pawn = other.gameObject.GetComponent<Pawn>();
		if (pawn != null && type == pawn.Type && player == pawn.player)
		{
			PerformAction();
		}
	}

	protected abstract void PerformAction();
}
