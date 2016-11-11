public class MapGeneratedEvent : TinyMessageBase
{
	public Tile[,] Map;
	public MapGeneratedEvent(object sender) : base(sender)
	{

	}
}