using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PawnEvent : UnityEvent<Pawn> { }

public class Board : MonoBehaviour {
	public float BoardLayerY = 0f;
	public float PawnsLayerY = 0.5f;
	public float DoorsLayerY = 0.75f;

	public PawnEvent OnPawnDestroyedEvent;

	public ICollection<GridCell> Cells { get { return cells.Values; } }
	public ICollection<Pawn> Pawns {  get { return pawns.Values; } }

	private Dictionary<Vector3, GridCell> cells = new Dictionary<Vector3, GridCell>();
	private Dictionary<Vector3, Pawn> pawns = new Dictionary<Vector3, Pawn>();
	private Dictionary<Vector3, Door> doors = new Dictionary<Vector3, Door>();

	public GridCell GetCell(Vector3 position)
	{
		Vector3 snapped = Unit.SnapPosition(position, BoardLayerY);
		GridCell cell;
		bool cellFound = cells.TryGetValue(snapped, out cell);
		return cellFound ? cell : null;
	}

	public Pawn GetPawn(GridCell cell)
	{
		if (cell == null)
		{
			return null;
		}

		Pawn pawn;
		Vector3 pos = Unit.SnapPosition(cell.transform.position, PawnsLayerY);
		bool hasPawn = pawns.TryGetValue(pos, out pawn);
		return hasPawn ? pawn : null;
	}

	public Door GetDoor(GridCell cell)
	{
		if (cell == null)
		{
			return null;
		}

		Door door;
		Vector3 pos = Unit.SnapPosition(cell.transform.position, DoorsLayerY);
		bool hasDoor = doors.TryGetValue(pos, out door);
		return hasDoor ? door : null;
	}

	public bool IsFree(GridCell cell)
	{
		if (cell == null)
		{
			return false;
		}

		return GetPawn(cell) == null && GetDoor(cell) == null;
	}

	public bool IsNotFree(GridCell cell)
	{
		return !IsFree(cell);
	}

	// Use this for initialization
	void Start ()
	{
		initializeDict(cells, BoardLayerY);
		initializeDict(pawns, PawnsLayerY);
		initializeDict(doors, DoorsLayerY);
		updatePawnsMaterial();

		Debug.Log(cells.Count + " cells found");
		Debug.Log(pawns.Count + " pawns found");
		Debug.Log(doors.Count + " doors found");
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
		// FIXME use another dict to store last object position?
		// kind of BiDiMap?
		bool updated = false;

		if (entity is Pawn)
		{
			updated |= TryUpdatePosition(entity, pawns);
		}
		if (entity is Door)
		{
			updated |= TryUpdatePosition(entity, doors);
		}
		if (entity is GridCell)
		{
			updated |= TryUpdatePosition(entity, cells);
		}
		
		if (!updated)
		{
			Debug.LogError("Failed to update unit at position: " + entity.transform.position);
		}
	}

	private void updateDestroyed(Unit entity)
	{
		Vector3 snappedPosition = Unit.SnapPosition(entity.Target);
		if (entity is Pawn)
		{
			OnPawnDestroyedEvent.Invoke(entity as Pawn);
			pawns.Remove(snappedPosition);
		}
		else if (entity is Door)
		{
			Debug.Log("Remove door: " + snappedPosition);
			Debug.Log("doors nb: " + doors.Count);
			doors.Remove(snappedPosition);
			Debug.Log("doors nb: " + doors.Count);
		}
		else if (entity is GridCell)
		{
			cells.Remove(snappedPosition);
		}
	}

	private static bool TryUpdatePosition<T>(Unit entity, Dictionary<Vector3, T> dict) where T : Unit
	{
		T unit;
		Vector3 pos = Unit.SnapPosition(entity.PreviousTarget);
		if (dict.TryGetValue(pos, out unit))
		{
			dict.Remove(pos);
			dict[entity.Target] = unit;
			return true;
		}
		return false;
	}
	
}
