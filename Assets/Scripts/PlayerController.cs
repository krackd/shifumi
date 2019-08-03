using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Players")]
	public Player[] Players;

	[Header("Selection")]
	public Color SelectedPawnColor = Color.blue;
	public LayerMask SelectableLayer;
	public float PathFindingStepFactor = 0.75f;

	[Header("Player Turn Text")]
	public TextMeshPro PlayerTurnText;

	private int playerIndex = 0;
	private Player Player { get { return Players[playerIndex]; } }
	private bool IsSoloGame { get { return Players.Length == 1; } }
	private bool IsMultiplayerGame { get { return Players.Length > 1; } }

	private GameManager gm;

	public Pawn SelectedPawn {
		get { return selectedPawn; }
		set
		{
			UpdateSelectedPawn(value);
		}
	}
	[SerializeField] private Pawn selectedPawn;
	private bool updatePawnOutline = true;
	private Outline prevOutline;

	// Use this for initialization
	void Start () {
		gm = GameService.FindGameManager();
		UpdatePlayerTurnColor();
		GameService.FindBoard().OnPawnDestroyedEvent.AddListener(OnPawnDestroyed);
	}

	// Update is called once per frame
	void Update()
	{
		UpdateOutline();
		UpdateTurn();
	}

	private void UpdateTurn()
	{
		bool isTurnDone = UpdateMouseInput();
		if (isTurnDone)
		{
			UpdatePlayerIndex();
		}
	}

	private void UpdateOutline()
	{
		if (updatePawnOutline)
		{
			UpdateSelectedPawnOutline();
			updatePawnOutline = false;
		}
	}

	private void UpdateSelectedPawn(Pawn value)
	{
		if (selectedPawn != null) {
			selectedPawn.Outline.enabled = false;
			selectedPawn.RestoreOutileColor();
		}
		selectedPawn = value;
		UpdateSelectedPawnOutline();
	}

	private void UpdateSelectedPawnOutline()
	{
		if (selectedPawn != null)
		{
			selectedPawn.Outline.enabled = true;
			selectedPawn.Outline.OutlineColor = SelectedPawnColor;
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

	private bool UpdateMouseInput()
	{
		GameObject underMousGo = FindGameObjectUnderMouse();
		Highlight(underMousGo);
		return UpdateSelection(underMousGo);
	}

	private bool UpdateSelection(GameObject underMousGo)
	{
		bool isTurnDone = false;

		if (Input.GetButtonDown("Fire1"))
		{
			Pawn pawn = underMousGo != null ? underMousGo.GetComponent<Pawn>() : null;
			if (pawn != null && IsPlayerPawn(pawn))
			{
				SelectedPawn = pawn;
			}
			else if (SelectedPawn != null)
			{
				isTurnDone = MoveOrAttack(underMousGo);
			}
		}
		
		return isTurnDone;
	}

	private bool MoveOrAttack(GameObject underMousGo)
	{
		bool isTurnDone = false;

		Pawn pawnRelease = underMousGo != null ? underMousGo.GetComponent<Pawn>() : null;
		GridCell cellRelease = underMousGo != null ? underMousGo.GetComponent<GridCell>() : null;
		if (pawnRelease != null && IsEnemyPawn(pawnRelease) && CanReach(SelectedPawn, pawnRelease) && SelectedPawn.CanBeat(pawnRelease))
		{
			SelectedPawn.Attack(pawnRelease);
			isTurnDone = true;
		}
		else if (cellRelease != null && gm.Board.IsFree(cellRelease) && CanReach(SelectedPawn, cellRelease))
		{
			SelectedPawn.Move(cellRelease);
			isTurnDone = true;
		}

		return isTurnDone;
	}

	private void Highlight(GameObject underMousGo)
	{
		Pawn pawn = underMousGo != null ? underMousGo.GetComponent<Pawn>() : null;
		if (SelectedPawn != null && pawn == SelectedPawn)
		{
			return;
		}

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
		Vector3 aPos = Unit.SnapPosition(a.transform.position, gm.Board.BoardLayerY);
		Vector3 bPos = Unit.SnapPosition(b.transform.position, gm.Board.BoardLayerY);
		Vector3 start = aPos;
		Vector3 diff = bPos - aPos;
		float diffSqrMag = diff.sqrMagnitude;
		Vector3 direction = bPos - aPos;
		Vector3 end = aPos + direction * PathFindingStepFactor;
		direction.Normalize();

		Vector3 offset = direction;
		while (offset.sqrMagnitude < diffSqrMag)
		{
			GridCell cell = gm.Board.GetCell(start + offset);
			if (cell == null || !gm.Board.IsFree(cell))
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

	private void OnPawnDestroyed(Pawn pawn)
	{
		if (IsSoloGame && pawn.player == Players[0])
		{
			int nbPawns = gm.Board.Pawns
				.Where(p => p.player == Players[0])
				.Count();

			if (nbPawns <= 0)
			{
				gm.Loose();
			}
		}
	}
}
