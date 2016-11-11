public class MeshGeneratedEvent : TinyMessageBase
{
	public Tile[,] Map;
	public MeshGeneratedEvent(object sender) : base(sender)
	{

	}
}