using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public Board Board { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
		Board = GameService.FindBoard();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
		{
			SceneManager.LoadScene("Main Menu");
		}
    }
}
