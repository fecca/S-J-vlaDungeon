using UnityEngine;

public class MessageHub : MonoBehaviour
{
	public static ITinyMessengerHub Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = new TinyMessengerHub();
		}
		else
		{
			Debug.Log("Instance already declared, that's not right. Fix it or suffer the consequences...");
		}
	}
}