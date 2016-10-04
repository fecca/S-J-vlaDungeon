using UnityEngine;

public class ControlNode : Node
{
	public bool Active;
	public Node AboveNode;
	public Node RightNode;

	public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
	{
		Active = active;
		AboveNode = new Node(position + Vector3.forward * squareSize / 2f);
		RightNode = new Node(position + Vector3.right * squareSize / 2f);
	}
}