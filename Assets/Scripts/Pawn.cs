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
		spritePlane = GetComponentInChildren<SpritePlane>();
		pawnModel = GetComponentInChildren<PawnModel>();

		Renderer renderer = pawnModel.GetComponent<Renderer>();
		Material pawnmat = getPawnMaterial();
		pawnmat.color = player.PlayerColor;
		renderer.sharedMaterial = pawnmat;

		resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
	}

	private Material getPawnMaterial()
	{
		Material pawnMaterial = player.PawnMaterial;
		if (pawnMaterial == null)
		{
			Renderer renderer = pawnModel.GetComponent<Renderer>();
			pawnMaterial = Instantiate(renderer.sharedMaterial);
			player.PawnMaterial = pawnMaterial;
		}
		return pawnMaterial;
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	public void Attack(Pawn other)
	{
		if (isOpponent(other) && CanBeat(other))
		{
			// Destorying the opponent and take his place
			type = other.type;
			UpdateMaterial();
			Target = other.transform.position;
			Destroy(other.gameObject);
		}
	}

	internal void UpdateMaterial()
	{
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
}
