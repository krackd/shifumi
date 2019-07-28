using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public float SmoothMoveSpeed = 5f;
	public float TargetReachedThresold = 0.0001f;

	public Vector3 Target { get { return targetPosition; } set { targetPosition = value; shouldMoveToTarget = true; } }
	private Vector3 targetPosition;
	private bool shouldMoveToTarget = false;

	public virtual void Start()
	{
		targetPosition = transform.position;
	}

	public virtual void Update()
	{
		if (shouldMoveToTarget)
		{
			MoveToTarget();
		}
	}

	private void MoveToTarget()
	{
		bool targetReached = IsTargetReached();
		if (targetReached)
		{
			SetTargetReached();
		}
		else
		{
			OneStepToTarget();
		}
	}

	private bool IsTargetReached()
	{
		return Vector3.SqrMagnitude(transform.position - targetPosition) < TargetReachedThresold;
	}

	private void SetTargetReached()
	{
		Debug.Log("Target reached!");
		transform.position = targetPosition;
		shouldMoveToTarget = false;
	}

	private void OneStepToTarget()
	{
		transform.position = Vector3.Lerp(transform.position, targetPosition, SmoothMoveSpeed * Time.deltaTime);
	}

	public void JumpToPosition(Vector3 pos)
	{
		transform.position = pos;
		targetPosition = pos;
		shouldMoveToTarget = false;
	}

	public void Snap(float layer)
	{
		Vector3 snappedPosition = SnapPosition(transform.position, layer);
		JumpToPosition(snappedPosition);
	}

	public static Vector3 SnapPosition(Vector3 pos, float layer)
	{
		Vector3 snapped = pos;
		snapped.x = (int)pos.x;
		snapped.y = layer;
		snapped.z = (int)pos.z;
		return snapped;
	}
}
