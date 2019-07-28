using UnityEngine;

public class Player : MonoBehaviour {

	public Color PlayerColor = Color.green;

	public bool IsSamePlayer(Player other)
	{
		return Equals(other);
	}
}
