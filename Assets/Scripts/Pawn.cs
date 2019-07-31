﻿using UnityEngine;

public class Pawn : Unit {

	public enum Type
	{
		CISOR,
		LEAF,
		BOMB,
		WELL,
		ROCK
	}
	public Type type;

	public Player player;

	private SpritePlane spritePlane;
	private PawnModel pawnModel;

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
			Target = other.transform.position;
			Destroy(other.gameObject);
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
			case Type.BOMB:
				return other.type == Type.WELL || other.type == Type.LEAF;
			case Type.CISOR:
				return other.type == Type.BOMB || other.type == Type.LEAF;
			case Type.ROCK:
				return other.type == Type.CISOR || other.type == Type.BOMB;
			case Type.WELL:
				return other.type == Type.ROCK || other.type == Type.CISOR;
			case Type.LEAF:
				return other.type == Type.WELL || other.type == Type.ROCK;
		}
		return false;
	}
}
