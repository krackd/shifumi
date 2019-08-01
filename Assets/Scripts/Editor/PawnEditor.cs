using System;
using UnityEditor;

[CustomEditor(typeof(Pawn))]
[CanEditMultipleObjects]
public class PawnEditor : Editor
{
	private Pawn[] pawns;
	private SerializedObject[] soPawns;

	private void OnEnable()
	{
		int i = 0;
		pawns = new Pawn[targets.Length];
		Array.ForEach(targets, target => pawns[i++] = (Pawn)target);

		soPawns = new SerializedObject[targets.Length];
		i = 0;
		Array.ForEach(targets, target => soPawns[i++] = new SerializedObject(target));
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();

		if (EditorGUI.EndChangeCheck())
		{
			Array.ForEach(soPawns, so => so.ApplyModifiedProperties());
			Array.ForEach(pawns, pawn =>
			{
				pawn.UpdatePawnColor();
				pawn.UpdateMaterial();
			});
		}
	}
}
