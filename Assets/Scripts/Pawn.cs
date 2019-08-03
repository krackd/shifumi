using System;
using UnityEngine;

public class Pawn : Unit {

	public enum PawnType
	{
		CISOR,
		LEAF,
		BOMB,
		WELL,
		ROCK
	}

	public PawnType Type
	{
		get { return type; }
		set
		{
			type = value;
			UpdateMaterial();
		}
	}
	[SerializeField] private PawnType type;

	public Player player;

	private SpritePlane spritePlane;
	private PawnModel pawnModel;

	private ResourcesManager resourcesManager;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		findChildrenGameObjects();
		findResourcesManager();

		UpdatePawnColor();
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	/// <summary>
	/// Attack the other pawn and moves the opponent.
	/// </summary>
	/// <param name="other">The other pawn</param>
	/// <returns>True if the attack leads to a success, false otherwise.</returns>
	public bool Attack(Pawn other)
	{
		return Attack(other, true);
	}

	/// <summary>
	/// Attack the other pawn.
	/// </summary>
	/// <param name="other">The other pawn</param>
	/// <param name="moveToOpponent">True if should move to opponent  position after attack, false otherwise</param>
	/// <returns>True if the attack leads to a success, false otherwise.</returns>
	private bool Attack(Pawn other, bool moveToOpponent)
	{
		if (!isOpponent(other))
		{
			return false;
		}

		if (CanBeat(other))
		{
			// Destorying the opponent and take his place
			type = other.type;
			UpdateMaterial();
			Vector3 otherPos = other.transform.position;
			Vector3 snappedPosition = SnapPosition(otherPos, otherPos.y);
			Destroy(other.gameObject);
			if (moveToOpponent) { Target = snappedPosition; }
			return true;
		}
		// loose against opponent = get eat
		else if (other.CanBeat(this))
		{
			other.Attack(this, false);
		}
		return false;
	}

	public void UpdateMaterial()
	{
		// This method shall be used from inspector,
		// so we have to ensure that attributes before using them
		ensureAttributes();

		switch (type)
		{
			case Pawn.PawnType.BOMB:
				SetMaterial(resourcesManager.BombMat);
				break;
			case Pawn.PawnType.LEAF:
				SetMaterial(resourcesManager.LeafMat);
				break;
			case Pawn.PawnType.WELL:
				SetMaterial(resourcesManager.WellMat);
				break;
			case Pawn.PawnType.ROCK:
				SetMaterial(resourcesManager.RockMat);
				break;
			case Pawn.PawnType.CISOR:
				SetMaterial(resourcesManager.CisorMat);
				break;
		}
	}

	public void Move(GridCell cell)
	{
		if (cell == null)
		{
			return;
		}

		Target = SnapPosition(cell.transform.position, transform.position.y);
	}

	public void SetMaterial(Material mat)
	{
		Renderer meshRenderer = spritePlane.GetComponent<Renderer>();
		meshRenderer.sharedMaterial = mat;
	}

	private bool isOpponent(Pawn other)
	{
		return !player.IsSamePlayer(other.player);
	}

	public bool CanBeat(Pawn other)
	{
		switch (type)
		{
			case PawnType.BOMB:
				return other.type == PawnType.WELL || other.type == PawnType.LEAF;
			case PawnType.CISOR:
				return other.type == PawnType.BOMB || other.type == PawnType.LEAF;
			case PawnType.ROCK:
				return other.type == PawnType.CISOR || other.type == PawnType.BOMB;
			case PawnType.WELL:
				return other.type == PawnType.ROCK || other.type == PawnType.CISOR;
			case PawnType.LEAF:
				return other.type == PawnType.WELL || other.type == PawnType.ROCK;
		}
		return false;
	}

	public void UpdatePawnColor()
	{
		ensureAttributes();

		Renderer renderer = pawnModel.GetComponent<Renderer>();
		Material pawnmat = getPawnMaterial();
		if (pawnmat != null)
		{
			pawnmat.color = player.PlayerColor;
			renderer.sharedMaterial = pawnmat;
		}
	}

	private Material getPawnMaterial()
	{
		if (player == null)
		{
			return null;
		}

		Material pawnMaterial = player.PawnMaterial;
		if (pawnMaterial == null)
		{
			Renderer renderer = pawnModel.GetComponent<Renderer>();
			pawnMaterial = Instantiate(renderer.sharedMaterial);
			player.PawnMaterial = pawnMaterial;
		}
		return pawnMaterial;
	}

	private void ensureAttributes()
	{
		if (isMissingResourcesManager())
		{
			findResourcesManager();
		}

		if (areMissingChildrenGameObjects())
		{
			findChildrenGameObjects();
		}
	}

	private bool isMissingResourcesManager()
	{
		return resourcesManager == null;
	}

	private void findResourcesManager()
	{
		resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
	}

	private bool areMissingChildrenGameObjects()
	{
		return spritePlane == null || pawnModel == null;
	}

	private void findChildrenGameObjects()
	{
		spritePlane = GetComponentInChildren<SpritePlane>();
		pawnModel = GetComponentInChildren<PawnModel>();
	}
}
