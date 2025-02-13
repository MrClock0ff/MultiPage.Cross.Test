using MultiPage.Cross.Test;

namespace MultiPage.Cross.Android.Test;

public class DataSource
{
	private IEnumerable<IEnumerable<SomeItem>> _chunks;

	public DataSource()
	{
		Reload(null);
	}

	public int ItemCount
	{
		get
		{
			return _chunks.Count();
		}
	}

	public void Reload(IEnumerable<SomeItem>? items)
	{
		IEnumerable<IEnumerable<SomeItem>>? chunks = null;
		items?.TrySlice(6, out chunks);
		_chunks = chunks ?? new List<IEnumerable<SomeItem>>();
	}

	public IEnumerable<SomeItem>? GetItem(int index)
	{
		return _chunks.ElementAtOrDefault(index);
	}
}