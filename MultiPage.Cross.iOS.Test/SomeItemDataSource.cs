namespace MultiPage.Cross.Test;

public class SomeItemDataSource : UIPageViewControllerDataSource, IUIPageViewControllerDelegate
{
	private readonly List<List<SomeItem>> _someItemChunks;
	private int _currentIndex;

	public SomeItemDataSource(IEnumerable<SomeItem> someItems)
	{
		List<List<SomeItem>>? someItemChunks = null;
		
		if (someItems.TrySlice(6, out IEnumerable<IEnumerable<SomeItem>>? chunks))
		{
			someItemChunks = chunks?.Select(e => e.ToList()).ToList();
		}
		
		_someItemChunks = someItemChunks ?? [];
	}

	public UIViewController? CurrentViewController => GetCurrentViewController();

	public override UIViewController GetNextViewController(UIPageViewController pageViewController,
		UIViewController referenceViewController)
	{
		return new UIViewController();
	}

	public override UIViewController GetPreviousViewController(UIPageViewController pageViewController,
		UIViewController referenceViewController)
	{
		return new UIViewController();
	}

	public override nint GetPresentationCount(UIPageViewController pageViewController)
	{
		return _someItemChunks.Count;
	}

	public override nint GetPresentationIndex(UIPageViewController pageViewController)
	{
		return _currentIndex;
	}

	private UIViewController? GetCurrentViewController()
	{
		if (_someItemChunks.Count == 0)
		{
			return null;
		}

		return new UIViewController();
	}
}