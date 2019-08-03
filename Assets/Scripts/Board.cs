using System;
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

	private BiDiMap<Vector3, GridCell> cells = new BiDiMap<Vector3, GridCell>();
	private BiDiMap<Vector3, Pawn> pawns = new BiDiMap<Vector3, Pawn>();
	private BiDiMap<Vector3, Door> doors = new BiDiMap<Vector3, Door>();

	public GridCell GetCell(Vector3 position)
	{
		Vector3 snapped = Unit.SnapPosition(position, BoardLayerY);
		GridCell cell;
		bool cellFound = cells.TryGetByFirst(snapped, out cell);
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
		bool hasPawn = pawns.TryGetByFirst(pos, out pawn);
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
		bool hasDoor = doors.TryGetByFirst(pos, out door);
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

	private void initializeDict<T>(BiDiMap<Vector3, T> dict, float layer) where T : Unit
	{
		T[] units = GetComponentsInChildren<T>();
		foreach (T unit in units)
		{
			unit.Snap(layer);
			Vector3 snapped = unit.transform.position;
			T previousCell;
			bool previousCellFound = dict.TryGetByFirst(snapped, out previousCell);
			if (previousCellFound)
			{
				Debug.Log("Destroying " + previousCell.gameObject);
				Destroy(previousCell.gameObject);
			}
			dict.Add(snapped, unit);
			unit.OnTargetChangedEvent.AddListener(updatePosition);
			unit.OnDestroyEvent.AddListener(updateDestroyed);
		}
	}

	private void updatePosition(Unit entity)
	{
		if (entity is Pawn)
		{
			TryUpdatePosition(entity as Pawn, pawns);
		}
		if (entity is Door)
		{
			TryUpdatePosition(entity as Door, doors);
		}
		if (entity is GridCell)
		{
			TryUpdatePosition(entity as GridCell, cells);
		}
	}

	private void updateDestroyed(Unit entity)
	{
		if (entity is Pawn)
		{
			OnPawnDestroyedEvent.Invoke(entity as Pawn);
			pawns.RemoveBySecond(entity as Pawn);
		}
		else if (entity is Door)
		{
			doors.RemoveBySecond(entity as Door);
		}
		else if (entity is GridCell)
		{
			cells.RemoveBySecond(entity as GridCell);
		}
	}

	private static bool TryUpdatePosition<T>(T entity, BiDiMap<Vector3, T> dict) where T : Unit
	{
		try
		{
			dict.RemoveBySecond(entity);
		}
		catch (ArgumentException)
		{
			// Do nothing if already missing
		}

		return dict.TryAdd(entity.Target, entity);
	}
	
}
