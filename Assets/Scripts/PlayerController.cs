using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Players")]
	public Player[] Players;

	[Header("Selection")]
	public LayerMask SelectableLayer;
	public float PathFindingStepFactor = 0.75f;

	[Header("Player Turn Text")]
	public TextMeshPro PlayerTurnText;

	private int playerIndex = 0;
	private Player Player { get { return Players[playerIndex]; } }
	private bool IsSoloGame { get { return Players.Length == 1; } }
	private bool IsMultiplayerGame { get { return Players.Length > 1; } }

	private Board board;

	private Pawn pawnPressed;
	private Outline prevOutline;

	// Use this for initialization
	void Start () {
		board = GameObject.Find("Board").GetComponent<Board>();
		UpdatePlayerTurnColor();
	}

	// Update is called once per frame
	void Update()
	{
		bool isTurnDone = UpdateTurn();
		if (isTurnDone)
		{
			UpdatePlayerIndex();
		}
	}

	private void UpdatePlayerIndex()
	{
		playerIndex++;
		playerIndex %= Players.Length;
		UpdatePlayerTurnColor();
	}

	private void UpdatePlayerTurnColor()
	{
		if (PlayerTurnText != null)
		{
			PlayerTurnText.faceColor = Player.PlayerColor;
		}	
	}

	private bool UpdateTurn()
	{
		bool isTurnDone = false;

		GameObject underMousGo = FindGameObjectUnderMouse();
		Highlight(underMousGo);

		if (Input.GetButtonDown("Fire1"))
		{
			Pawn pawn = underMousGo != null ? underMousGo.GetComponent<Pawn>() : null;
			pawnPressed = pawn != null && IsPlayerPawn(pawn) ? pawn : null;
		}
		else if (pawnPressed != null && Input.GetButtonUp("Fire1"))
		{
			Pawn pawnRelease = underMousGo != null ? underMousGo.GetComponent<Pawn>() : null;
			GridCell cellRelease = underMousGo != null ? underMousGo.GetComponent<GridCell>() : null;
			if (pawnRelease != null && IsEnemyPawn(pawnRelease) && CanReach(pawnPressed, pawnRelease) && pawnPressed.CanBeat(pawnRelease))
			{
				pawnPressed.Attack(pawnRelease);
				isTurnDone = true;
			}
			else if (cellRelease != null && board.IsFree(cellRelease) && CanReach(pawnPressed, cellRelease))
			{
				pawnPressed.Move(cellRelease);
				isTurnDone = true;
			}

			pawnPressed = null;
		}

		return isTurnDone;
	}

	private void Highlight(GameObject underMousGo)
	{
		if (prevOutline != null)
		{
			prevOutline.enabled = false;
		}

		Outline outline = underMousGo != null ? underMousGo.GetComponent<Outline>() : null;
		if (outline != null)
		{
			outline.enabled = true;
			prevOutline = outline;
		}
	}

	private bool CanReach(Unit a, Unit b)
	{
		return IsMultiplayerGame
			? Unit.IsDistanceOne(a, b)
			: IsPathFree(a, b);
	}

	private bool IsPathFree(Unit a, Unit b)
	{
		Vector3 aPos = Unit.SnapPosition(a.transform.position, board.BoardLayerY);
		Vector3 bPos = Unit.SnapPosition(b.transform.position, board.BoardLayerY);
		Vector3 start = aPos;
		Vector3 diff = bPos - aPos;
		float diffSqrMag = diff.sqrMagnitude;
		Vector3 direction = bPos - aPos;
		Vector3 end = aPos + direction * PathFindingStepFactor;
		direction.Normalize();

		Vector3 offset = direction;
		while (offset.sqrMagnitude < diffSqrMag)
		{
			GridCell cell = board.GetCell(start + offset);
			if (cell == null || !board.IsFree(cell))
			{
				return false;
			}
			offset += direction;
		}
		return true;
	}

	private GameObject FindGameObjectUnderMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, SelectableLayer.value);
		if (hits.Length > 0)
		{
			hits = hits.Where(hit => SelectableLayer == (SelectableLayer | (1 << hit.collider.gameObject.layer))).ToArray();
			Array.Sort(hits, (a, b) => (int)(a.distance * 1000 - b.distance * 1000));
			return hits[0].collider.gameObject;
		}

		return null;
	}

	private bool IsPlayerPawn(Pawn pawn)
	{
		return Player.IsSamePlayer(pawn.player);
	}

	private bool IsEnemyPawn(Pawn pawn)
	{
		return !Player.IsSamePlayer(pawn.player);
	}
}
