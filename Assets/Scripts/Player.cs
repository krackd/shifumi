using UnityEngine;

public class Player : MonoBehaviour {

	public Color PlayerColor = Color.green;
	public Material PawnMaterial;

	public bool IsSamePlayer(Player other)
	{
		return Equals(other);
	}
}
