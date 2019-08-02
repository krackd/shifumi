using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameService
{
    
	public static GameManager FindGameManager()
	{
		return GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	public static Board FindBoard()
	{
		return GameObject.Find("Board").GetComponent<Board>();
	}
}
