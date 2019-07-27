using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	private const float BOARD_LAYER = 0f;
	private const float PAWNS_LAYER = 0.5f;

	private Dictionary<Vector3, GridCell> board = new Dictionary<Vector3, GridCell>();
	private Dictionary<Vector3, Pawn> pawns = new Dictionary<Vector3, Pawn>();

	public GridCell GetCell(Vector3 position)
	{
		Vector3 snapped = snapPosition(position, 0);
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
		initializeDict(board, BOARD_LAYER);
		initializeDict(pawns, PAWNS_LAYER);

		Debug.Log(board.Count + " grid cells found");
		Debug.Log(pawns.Count + " pawns found");
	}

	private void initializeDict<T>(Dictionary<Vector3, T> dict, float layer) where T : MonoBehaviour
	{
		T[] cells = GetComponentsInChildren<T>();
		foreach (T cell in cells)
		{
			Vector3 snappedPosition = snapPosition(cell.transform.position, layer);
			cell.transform.position = snappedPosition;
			T previousCell;
			bool previousCellFound = dict.TryGetValue(snappedPosition, out previousCell);
			if (previousCellFound)
			{
				Destroy(previousCell.gameObject);
			}
			dict[snappedPosition] = cell;
		}
	}

	private static Vector3 snapPosition(Vector3 pos, float layer)
	{
		Vector3 snapped = pos;
		snapped.x = (int)pos.x;
		snapped.y = layer;
		snapped.z = (int)pos.z;
		return snapped;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
