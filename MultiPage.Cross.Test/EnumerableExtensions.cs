namespace MultiPage.Cross.Test;

public static class EnumerableExtensions
{
	public static IEnumerable<IEnumerable<T>> Slice<T>(this IEnumerable<T> value, int chunkSize)
	{
		return value
			.Select((e, i) => new { Index = i, Value = e })
			.GroupBy(e => e.Index / chunkSize)
			.Select(g => g.Select(e => e.Value));
	}

	public static bool TrySlice<T>(this IEnumerable<T> value, int chunkSize, out IEnumerable<IEnumerable<T>>? sliced)
	{
		sliced = null;

		try
		{
			sliced = value.Slice(chunkSize);
			return true;
		}
		catch
		{
			return false;
		}
	}
}