using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnitEvent : UnityEvent<Unit> { }

public class Unit : MonoBehaviour
{
	public float SmoothMoveSpeed = 5f;
	public float TargetReachedThresold = 0.0001f;

	public UnitEvent OnTargetChangedEvent;
	public UnitEvent OnDestroyEvent;

	public Vector3 Target
	{
		get
		{
			return targetPosition;
		}

		set
		{
			targetPosition = value;
			shouldMoveToTarget = true;
			OnTargetChangedEvent.Invoke(this);
		}
	}

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

	private bool IsTargetReached()
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
		return SqrMagnitude(a, b, a.transform.position.y) == 1;
	}
}
