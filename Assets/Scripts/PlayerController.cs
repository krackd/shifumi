using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Players")]
	public Player[] Players;

	public LayerMask PawnLayer;

	private int playerIndex = 0;
	private Player Player { get { return Players[playerIndex]; } }

	private Board board;

	private Pawn pawnPressed;

	// Use this for initialization
	void Start () {
		board = GameObject.Find("Board").GetComponent<Board>();
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
		Debug.Log("playerIndex: " + playerIndex);
	}

	private bool UpdateTurn()
	{
		bool isTurnDone = false;

		if (Input.GetButtonDown("Fire1"))
		{
			GameObject go = FindGameObjectUnderMouse();
			Pawn pawn = go != null ? go.GetComponent<Pawn>() : null;
			pawnPressed = pawn != null && IsPlayerPawn(pawn) ? pawn : null;
		}
		else if (pawnPressed != null && Input.GetButtonUp("Fire1"))
		{
			GameObject go = FindGameObjectUnderMouse();
			Pawn pawnRelease = go.GetComponent<Pawn>();
			GridCell cellRelease = go.GetComponent<GridCell>();
			if (pawnRelease != null && IsEnemyPawn(pawnRelease) && Unit.IsDistanceOne(pawnPressed, pawnRelease))
			{
				pawnPressed.Attack(pawnRelease);
				isTurnDone = true;
			}
			else if (cellRelease != null && board.IsFree(cellRelease) && Unit.IsDistanceOne(pawnPressed, cellRelease))
			{
				pawnPressed.Move(cellRelease);
				isTurnDone = true;
			}

			pawnPressed = null;
		}

		return isTurnDone;
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

	private bool IsPlayerPawn(Pawn pawn)
	{
		return Player.IsSamePlayer(pawn.player);
	}

	private bool IsEnemyPawn(Pawn pawn)
	{
		return !Player.IsSamePlayer(pawn.player);
	}
}
