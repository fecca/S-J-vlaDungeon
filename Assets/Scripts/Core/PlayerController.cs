using System;
using UnityEngine;

public class PlayerController : Character
{
	public override GameObject GetGameObject()
	{
		throw new NotImplementedException();
	}

	public override Vector3 GetTransformPosition()
	{
		throw new NotImplementedException();
	}

	public override void InitializeAttacker(AttackData attackData)
	{
		throw new NotImplementedException();
	}

	public override void InitializeBrain()
	{
		throw new NotImplementedException();
	}

	public override void InitializeHealth(HealthData healthData)
	{
		throw new NotImplementedException();
	}

	public override void InitializeMover(MoveData moveData)
	{
		throw new NotImplementedException();
	}

	public override void InitializePathfindingAgent()
	{
		throw new NotImplementedException();
	}

	public override void TakeDamage()
	{
		throw new NotImplementedException();
	}

	void Start()
	{

	}

	void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			transform.Translate(transform.forward * 0.1f);
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.Translate(-transform.right * 0.1f);
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.Translate(-transform.forward * 0.1f);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.Translate(transform.right * 0.1f);
		}
	}
}
