public class MeshCreatedEvent : TinyMessageBase
{
	public Tile[,] Map;

	public MeshCreatedEvent(object sender) : base(sender)
	{

	}
}