using UnityEngine;

public class OpenDoorSwitch : Switch
{
	public GameObject Door;

	protected override void PerformAction()
	{
		if (Door != null)
		{
			Destroy(Door);
		}
	}
}
