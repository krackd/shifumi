public class WinConditionSwitch : Switch
{
	protected override void PerformAction()
	{
		GameManager.Win();
	}
}
