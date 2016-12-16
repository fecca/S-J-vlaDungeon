using UnityEngine;

interface ICharacterFactory
{
	Player CreatePlayer(GameObject prefab);
	Enemy CreateEnemy(GameObject prefab, HealthType healthType, AttackerType attackerType, MoverType moverType, PerceptionType perceptionType, ICharacter target);
	Enemy CreateRandomEnemy(GameObject prefab, ICharacter target);
}