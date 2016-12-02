public class MapCreatedEvent : TinyMessageBase
{
	public Tile[,] Map;

	public MapCreatedEvent(object sender) : base(sender)
	{

	}
}