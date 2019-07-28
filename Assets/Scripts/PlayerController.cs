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
			pawnPressed = FindGameObjectUnderMouse().GetComponent<Pawn>();
		}
		else if (pawnPressed != null && Input.GetButtonUp("Fire1"))
		{
			GameObject go = FindGameObjectUnderMouse();
			Pawn pawnRelease = go.GetComponent<Pawn>();
			GridCell cellRelease = go.GetComponent<GridCell>();
			if (pawnRelease != null && Unit.IsDistanceOne(pawnPressed, pawnRelease))
			{
				pawnPressed.Attack(pawnRelease);
			}
			else if (cellRelease != null && board.IsFree(cellRelease) && Unit.IsDistanceOne(pawnPressed, cellRelease))
			{
				pawnPressed.Move(cellRelease);
			}

			pawnPressed = null;
		}
	}

	private GameObject FindGameObjectUnderMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, PawnLayer.value))
		{
			return hit.collider.gameObject;
		}

		return null;
	}
}
