﻿using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnitEvent : UnityEvent<Unit> { }

[RequireComponent(typeof(Outline))]
public class Unit : MonoBehaviour
{
	public float SmoothMoveSpeed = 5f;
	public float TargetReachedThresold = 0.0001f;

	public UnitEvent OnTargetChangedEvent;
	public UnitEvent OnDestroyEvent;

	public Outline Outline { get; private set; }
	private Color outlineInitialColor;

	public Vector3 Target
	{
		get
		{
			return targetPosition;
		}

		set
		{
			PreviousTarget = targetPosition;
			targetPosition = value;
			shouldMoveToTarget = true;
			OnTargetChangedEvent.Invoke(this);
		}
	}

	public Vector3 PreviousTarget { get; private set; }

	protected GameManager GameManager { get; private set; }

	private Vector3 targetPosition;
	private bool shouldMoveToTarget = false;

	public virtual void Start()
	{
		targetPosition = transform.position;
		GameManager = GameService.FindGameManager();
		Outline = GetComponent<Outline>();
		outlineInitialColor = Outline.OutlineColor;
	}

	public virtual void Update()
	{
		if (shouldMoveToTarget)
		{
			MoveToTarget();
		}
	}

	private void OnDestroy()
	{
		OnDestroyEvent.Invoke(this);
		OnTargetChangedEvent.RemoveAllListeners();
		OnDestroyEvent.RemoveAllListeners();
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

	public bool IsTargetReached()
	{
		return Vector3.SqrMagnitude(transform.position - targetPosition) < TargetReachedThresold;
	}

	private void SetTargetReached()
	{
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

	public void RestoreOutileColor()
	{
		Outline.OutlineColor = outlineInitialColor;
	}

	public static Vector3 SnapPosition(Vector3 pos)
	{
		return SnapPosition(pos, pos.y);
	}

	public static Vector3 SnapPosition(Vector3 pos, float layer)
	{
		Vector3 snapped = pos;
		snapped.x = (int)pos.x;
		snapped.y = layer;
		snapped.z = (int)pos.z;
		return snapped;
	}

	public static float SqrMagnitude(Unit a, Unit b, float layer)
	{
		Vector3 posA = SnapPosition(a.transform.position, layer);
		Vector3 posB = SnapPosition(b.transform.position, layer);
		return Vector3.SqrMagnitude(posA - posB);
	}

	public static bool IsDistanceOne(Unit a, Unit b)
	{
		float sqrMag = SqrMagnitude(a, b, a.transform.position.y);
		return sqrMag >= 1 && sqrMag <= 2;
	}
}
