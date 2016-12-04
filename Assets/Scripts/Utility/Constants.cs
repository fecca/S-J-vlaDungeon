public class Constants
{
	public const int WallHeight = 4;
	public const int TileSize = 3;

	// -------- PATHFINDING DATA -------- //
	public const int HorizontalTileWeight = 10;
	public const int DiagonalTileWeight = 14;
	// -------- PATHFINDING DATA -------- //

	// -------- PLAYER DATA -------- //
	public const float PLAYER_HEALTH = 100;
	public const float PLAYER_PROJECTILE_SPEED = 20.0f;
	public const string PLAYER_PROJECTILE_NAME = "PlayerProjectile";
	public const float PLAYER_MOVEMENT_SPEED = 20.0f;
	// -------- PLAYER DATA -------- //

	// -------- ENEMY DATA -------- //
	public const float LOW_HEALTH = 1;
	public const float MEDIUM_HEALTH = 2;
	public const float HIGH_HEALTH = 3;

	public const float SLOW_TIME_BETWEEN_ATTACKS = 1.0f;
	public const float MEDIUM_TIME_BETWEEN_ATTACKS = 0.75f;
	public const float FAST_TIME_BETWEEN_ATTACKS = 0.5f;
	public const float SLOW_PROJECTILE_SPEED = 10.0f;
	public const float MEDIUM_PROJECTILE_SPEED = 20.0f;
	public const float FAST_PROJECTILE_SPEED = 30.0f;
	public const string ENEMY_PROJECTILE_NAME = "EnemyProjectile";

	public const float SLOW_POSITION_UPDATE_INTERVAL = 0.8f;
	public const float MEDIUM_POSITION_UPDATE_INTERVAL = 0.5f;
	public const float FAST_POSITION_UPDATE_INTERVAL = 0.2f;
	public const float SLOW_MOVEMENT_SPEED = 5.0f;
	public const float MEDIUM_MOVEMENT_SPEED = 10.5f;
	public const float FAST_MOVEMENT_SPEED = 15.0f;

	public const float LOW_PERCEPTION_UPDATE_INTERVAL = 0.15f;
	public const float MEDIUM_PERCEPTION_UPDATE_INTERVAL = 0.10f;
	public const float HIGH_PERCEPTION_UPDATE_INTERVAL = 0.05f;
	public const float LOW_INNER_RADIUS = 5.0f;
	public const float MEDIUM_INNER_RADIUS = 10.0f;
	public const float HIGH_INNER_RADIUS = 15.0f;
	public const float LOW_OUTER_RADIUS = 15.0f;
	public const float MEDIUM_OUTER_RADIUS = 20.0f;
	public const float HIGH_OUTER_RADIUS = 25.0f;
	// -------- ENEMY DATA -------- //
}