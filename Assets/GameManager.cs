using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public Board Board { get; private set; }

	[SerializeField] private TextMeshProUGUI youWin;
	[SerializeField] private TextMeshProUGUI youLoose;

	// Start is called before the first frame update
	void Start()
    {
		Board = GameService.FindBoard();

		youWin.gameObject.SetActive(false);
		youLoose.gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
		{
			SceneManager.LoadScene("Main Menu");
		}
    }

	public void Win()
	{
		youWin.gameObject.SetActive(true);
	}

	public void Loose()
	{
		youLoose.gameObject.SetActive(true);
	}
}
