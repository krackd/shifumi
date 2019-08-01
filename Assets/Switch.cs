using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
	public Pawn.PawnType type;

	public UnityEvent OnSwitchTriggeredEvent;

	private void OnTriggerEnter(Collider other)
	{
		Pawn pawn = other.gameObject.GetComponent<Pawn>();
		if (pawn != null && type == pawn.Type)
		{
			Debug.Log("Triggered");
		}

		//if (layers == (layers | (1 << other.gameObject.layer)))
		//{
		//	OnSwitchTriggeredEvent.Invoke();
		//	Debug.Log("Triggered");
		//}
	}
}
