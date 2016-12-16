using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputHandler : MonoBehaviour, IInputHandler
{
	private IDictionary<KeyCode, List<TinyMessageBase>> _keyboardBindings = new Dictionary<KeyCode, List<TinyMessageBase>>();

	private void Awake()
	{
		ServiceLocator<IInputHandler>.Instance = this;
		SetupKeyboardBindings();
	}
	private void Update()
	{
		for (var i = 0; i < _keyboardBindings.Count; i++)
		{
			var binding = _keyboardBindings.ElementAt(i);
			if (Input.GetKeyDown(binding.Key))
			{
				for (int j = 0; j < binding.Value.Count; j++)
				{
					MessageHub.Instance.Publish(binding.Value[j]);
				}
			}
		}
	}
	private void SetupKeyboardBindings()
	{
		_keyboardBindings.Add(KeyCode.Return, new List<TinyMessageBase>
		{
			new DestroyGameEvent(null)
		});
	}

	public RaycastHit GetRaycastHit()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, 500f);
		return hit;
	}
}