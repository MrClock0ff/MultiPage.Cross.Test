namespace MultiPage.Cross.Test;

public class SomeItemService
{
	public Task<IEnumerable<SomeItem>> GetSomeItemsAsync(CancellationToken cancellationToken = default)
	{
		Random random = new Random();
		int maxItems = random.Next(5, 24);
		List<SomeItem> items = new();

		for (int i = 0; i < maxItems; i++)
		{
			SomeItem item = new SomeItem(Guid.NewGuid(), $"Item {i}", $"ItemIcon{i}");
			items.Add(item);
		}
		
		return Task.FromResult<IEnumerable<SomeItem>>(items);
	}
}