using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	public float BoardLayerY = 0f;
	public float PawnsLayerY = 0.5f;

	private Dictionary<Vector3, GridCell> board = new Dictionary<Vector3, GridCell>();
	private Dictionary<Vector3, Pawn> pawns = new Dictionary<Vector3, Pawn>();

	private ResourcesManager resourcesManager;

	public GridCell GetCell(Vector3 position)
	{
		Vector3 snapped = Unit.SnapPosition(position, 0);
		GridCell previousCell;
		bool previousCellFound = board.TryGetValue(snapped, out previousCell);
		if (previousCellFound)
		{
			return previousCell;
		}
		return null;
	}

	public bool IsFree(GridCell cell)
	{
		Pawn pawn;
		Vector3 pos = Unit.SnapPosition(cell.transform.position, PawnsLayerY);
		bool hasPawn = pawns.TryGetValue(pos, out pawn);
		return !hasPawn;
	}

	// Use this for initialization
	void Start ()
	{
		resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();

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
			Pawn.Type type = pawn.type;
			switch (type)
			{
				case Pawn.Type.BOMB:
					pawn.SetMaterial(resourcesManager.BombMat);
					break;
				case Pawn.Type.LEAF:
					pawn.SetMaterial(resourcesManager.LeafMat);
					break;
				case Pawn.Type.WELL:
					pawn.SetMaterial(resourcesManager.WellMat);
					break;
				case Pawn.Type.ROCK:
					pawn.SetMaterial(resourcesManager.RockMat);
					break;
				case Pawn.Type.CISOR:
					pawn.SetMaterial(resourcesManager.CisorMat);
					break;
			}
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
		TryUpdatePosition(entity, board);
		TryUpdatePosition(entity, pawns);
	}

	private void updateDestroyed(Unit entity)
	{
		TryRemove(entity, board);
		TryRemove(entity, pawns);
	}

	private static void TryUpdatePosition<T>(Unit entity, Dictionary<Vector3, T> dict) where T : Unit
	{
		T unit;
		Vector3 pos = entity.transform.position;
		if (dict.TryGetValue(pos, out unit))
		{
			dict.Remove(pos);
			dict[entity.Target] = unit;
		}
	}

	private static void TryRemove<T>(Unit entity, Dictionary<Vector3, T> dict) where T : Unit
	{
		T unit;
		Vector3 pos = entity.transform.position;
		if (dict.TryGetValue(pos, out unit))
		{
			dict.Remove(pos);
		}
	}
}
