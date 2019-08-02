using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class Enemy : MonoBehaviour
{
	[Header("Patrol")]
	public bool IsPatrolling = false;
	public Vector3 PatrolDirection = Vector3.forward;
	public float PatrolDelayInSecs = 0.5f;

	private float patrolTime = 0;
	private float patrolSign = 1;

	private Pawn pawn;
	private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
		gm = GameService.FindGameManager();
		pawn = GetComponent<Pawn>();
		ResetPatrolTime();
	}

    // Update is called once per frame
    void Update()
	{
		UpdatePatrol();
	}

	private void UpdatePatrol()
	{
		if (IsPatrolling)
		{
			if (patrolTime <= 0)
			{
				GridCell cell = getNextCell();
				if (cell == null)
				{
					cell = InvertPatrolSign();
				}

				if (gm.Board.IsNotFree(cell))
				{
					bool success = pawn.Attack(gm.Board.GetPawn(cell));
					if (!success)
					{
						cell = InvertPatrolSign();
					}
				}

				pawn.Move(cell);
				ResetPatrolTime();
			}

			patrolTime -= Time.deltaTime;
		}
	}

	private GridCell InvertPatrolSign()
	{
		GridCell cell;
		patrolSign *= -1;
		cell = getNextCell();
		return cell;
	}

	private void ResetPatrolTime()
	{
		patrolTime = PatrolDelayInSecs;
	}

	private GridCell getNextCell()
	{
		Vector3 pos = transform.position;
		Vector3 translation = PatrolDirection * patrolSign;
		Vector3 newPos = pos + translation;
		return gm.Board.GetCell(newPos);
	}
}
