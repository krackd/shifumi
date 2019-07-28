using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public LayerMask PawnLayer;

	private Board board;

	private Pawn pawnPressed;

	// Use this for initialization
	void Start () {
		board = GameObject.Find("Board").GetComponent<Board>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			pawnPressed = FindPawnUnderMouse();
		}
		else if (pawnPressed != null && Input.GetButtonUp("Fire1"))
		{
			Pawn pawnRelease = FindPawnUnderMouse();
			if (pawnRelease != null)
			{
				pawnPressed.Attack(pawnRelease);
			}

			pawnPressed = null;
		}
	}

	private Pawn FindPawnUnderMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, PawnLayer))
		{
			return hit.collider.gameObject.GetComponent<Pawn>();
		}

		return null;
	}
}
