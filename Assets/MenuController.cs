using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	private GameObject mainMenu;
	private GameObject puzzleMenu;
	private GameObject versusMenu;
	private GameObject backButton;

	private void Start()
	{
		mainMenu = GameObject.Find("/Canvas/MainMenu");
		puzzleMenu = GameObject.Find("/Canvas/Puzzle");
		versusMenu = GameObject.Find("/Canvas/Versus");
		backButton = GameObject.Find("/Canvas/Back");

		ShowMainMenu();
	}

	public void ShowMainMenu()
	{
		HideAll();
		mainMenu.SetActive(true);
	}

	public void ShowPuzzleMenu()
	{
		HideAll();
		puzzleMenu.SetActive(true);
		ShowBackButton();
	}
	public void ShowVersusMenu()
	{
		HideAll();
		versusMenu.SetActive(true);
		ShowBackButton();
	}

	private void HideAll()
	{
		mainMenu.SetActive(false);
		puzzleMenu.SetActive(false);
		versusMenu.SetActive(false);
		HideBackButton();
	}

	private void HideBackButton()
	{
		backButton.SetActive(false);
	}
	
	private void ShowBackButton()
	{
		backButton.SetActive(true);
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
