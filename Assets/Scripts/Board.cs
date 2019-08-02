using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PawnEvent : UnityEvent<Pawn> { }

public class Board : MonoBehaviour {
	public float BoardLayerY = 0f;
	public float PawnsLayerY = 0.5f;

	public PawnEvent OnPawnDestroyedEvent;

	public ICollection<GridCell> Cells { get { return board.Values; } }
	public ICollection<Pawn> Pawns {  get { return pawns.Values; } }

	private Dictionary<Vector3, GridCell> board = new Dictionary<Vector3, GridCell>();
	private Dictionary<Vector3, Pawn> pawns = new Dictionary<Vector3, Pawn>();

	public GridCell GetCell(Vector3 position)
	{
		Vector3 snapped = Unit.SnapPosition(position, BoardLayerY);
		GridCell cell;
		bool cellFound = board.TryGetValue(snapped, out cell);
		return cellFound ? cell : null;
	}

	public Pawn GetPawn(GridCell cell)
	{
		Pawn pawn;
		Vector3 pos = Unit.SnapPosition(cell.transform.position, PawnsLayerY);
		bool hasPawn = pawns.TryGetValue(pos, out pawn);
		return hasPawn ? pawn : null;
	}

	public bool IsFree(GridCell cell)
	{
		return GetPawn(cell) == null;
	}

	public bool IsNotFree(GridCell cell)
	{
		return !IsFree(cell);
	}

	// Use this for initialization
	void Start ()
	{
		initializeDict(board, BoardLayerY);
		initializeDict(pawns, PawnsLayerY);
		updatePawnsMaterial();

		Debug.Log(board.Count + " grid cells found");
		Debug.Log(pawns.Count + " pawns found");
	}

	private void updatePawnsMaterial()
	{
		foreach (Pawn pawn in pawns.Values)
		{
			pawn.UpdateMaterial();
		}
	}

	private void initializeDict<T>(Dictionary<Vector3, T> dict, float layer) where T : Unit
	{
		T[] units = GetComponentsInChildren<T>();
		foreach (T unit in units)
		{
			unit.Snap(layer);
			Vector3 snapped = unit.transform.position;
			T previousCell;
			bool previousCellFound = dict.TryGetValue(snapped, out previousCell);
			if (previousCellFound)
			{
				Debug.Log("Destroying " + previousCell.gameObject);
				Destroy(previousCell.gameObject);
			}
			dict[snapped] = unit;
			unit.OnTargetChangedEvent.AddListener(updatePosition);
			unit.OnDestroyEvent.AddListener(updateDestroyed);
		}
	}

	private void updatePosition(Unit entity)
	{
		bool updated = false;
		updated |= TryUpdatePosition(entity, board);
		updated |= TryUpdatePosition(entity, pawns);

		if (!updated)
		{
			Debug.LogError("Failed to update unit at position: " + entity.transform.position);
		}
	}

	private void updateDestroyed(Unit entity)
	{
		if (entity is Pawn)
		{
			OnPawnDestroyedEvent.Invoke(entity as Pawn);
		}
	}

	private static bool TryUpdatePosition<T>(Unit entity, Dictionary<Vector3, T> dict) where T : Unit
	{
		T unit;
		Vector3 pos = Unit.SnapPosition(entity.transform.position);
		if (dict.TryGetValue(pos, out unit))
		{
			dict.Remove(pos);
			dict[entity.Target] = unit;
			return true;
		}
		return false;
	}

	private static bool TryRemove<T>(Unit entity, Dictionary<Vector3, T> dict) where T : Unit
	{
		T unit;
		Vector3 pos = Unit.SnapPosition(entity.transform.position);
		if (dict.TryGetValue(pos, out unit))
		{
			dict.Remove(pos);
			return true;
		}
		return false;
	}
}
