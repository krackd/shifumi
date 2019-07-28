using UnityEngine;

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
	
	private SpritePlane spritePlane;

	// Use this for initialization
	public override void Start () {
		base.Start();
		spritePlane = GetComponentInChildren<SpritePlane>();
	}

	// Update is called once per frame
	public override void Update () {
		base.Update();
	}
	
	public void Attack(Pawn other)
	{
		if (isBeating(other))
		{
			// Destorying the opponent and take his place
			Target = other.transform.position;
			Destroy(other.gameObject);
		}
	}

	public void SetMaterial(Material mat)
	{
		Renderer meshRenderer = spritePlane.GetComponent<Renderer>();
		meshRenderer.sharedMaterial = mat;
	}

	private bool isBeating(Pawn other)
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
