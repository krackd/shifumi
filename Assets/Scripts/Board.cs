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
		T[] cells = GetComponentsInChildren<T>();
		foreach (T cell in cells)
		{
			cell.Snap(layer);
			Vector3 snapped = cell.transform.position;
			T previousCell;
			bool previousCellFound = dict.TryGetValue(snapped, out previousCell);
			if (previousCellFound)
			{
				Debug.Log("Destroying " + previousCell.gameObject);
				Destroy(previousCell.gameObject);
			}
			dict[snapped] = cell;
		}
	}
}
