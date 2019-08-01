using UnityEditor;

[CustomEditor(typeof(Pawn))]
[CanEditMultipleObjects]
public class PawnEditor : Editor
{
	private Pawn pawn;
	private SerializedObject soPawn;

	private void OnEnable()
	{
		pawn = (Pawn)target;
		soPawn = new SerializedObject(target);
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();

		if (EditorGUI.EndChangeCheck())
		{
			soPawn.ApplyModifiedProperties();
			pawn.UpdatePawnColor();
			pawn.UpdateMaterial();
		}
	}
}
