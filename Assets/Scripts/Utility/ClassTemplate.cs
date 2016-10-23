// constants => members => properties
// constructors => Unity game loop => overridden methods => private methods => public methods
// private => protected => public

public class ClassTemplate
{
	private const int Constant = 0;
	private readonly int Readonly;

	[UnityEngine.SerializeField]
	private int PrivateSerializeField;

	private int _localMember;

	protected int protectedMember;

	public int GlobalMember;

	private int PrivateProperty { get; set; }

	protected int ProtectedProperty { get; set; }

	public int PublicProperty { get; set; }

	private ClassTemplate(int privateConstructor) { }
	public ClassTemplate() { }

	private void Update() { }

	public /*override*/ void OverriddenMethod() { }

	private void PrivateMethod() { }

	protected void ProtectedMethod() { }

	public void PublicMethod() { }
}