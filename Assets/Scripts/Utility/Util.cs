using System;

public static class Util
{
	public static T GetRandomEnumValue<T>()
	{
		var values = Enum.GetValues(typeof(T));
		var random = UnityEngine.Random.Range(0, values.Length);
		var value = values.GetValue(random);

		return (T)value;
	}
}