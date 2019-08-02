using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
	public Pawn.PawnType type;
	public Player player;

	public UnityEvent OnSwitchTriggeredEvent;

	private void OnTriggerEnter(Collider other)
	{
		if (player == null)
		{
			return;
		}

		Pawn pawn = other.gameObject.GetComponent<Pawn>();
		if (pawn != null && type == pawn.Type && player == pawn.player)
		{
			Debug.Log("Triggered");
		}
	}
}
