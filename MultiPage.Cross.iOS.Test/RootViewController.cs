using CoreFoundation;

namespace MultiPage.Cross.Test;

public class RootViewController : UIViewController
{
	private readonly SomeItemService _service;
	private DataSource _dataSource;
	private UIPageViewController? _pageViewController;

	public RootViewController()
	{
		_service = new SomeItemService();
	}

	public override async void ViewDidLoad()
	{
		base.ViewDidLoad();

		View!.BackgroundColor = UIColor.SystemBlue;

		UIPageViewController pageViewController = _pageViewController = 
			new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll,
			UIPageViewControllerNavigationOrientation.Horizontal);
		
		//pageViewController.View!.BackgroundColor = UIColor.Orange;
		pageViewController.WillMoveToParentViewController(this);
		View!.AddSubview(pageViewController.View!);
		AddChildViewController(pageViewController);
		pageViewController.DidMoveToParentViewController(this);
		
		pageViewController.View!.TranslatesAutoresizingMaskIntoConstraints = false;
		
		NSLayoutConstraint.ActivateConstraints(new []
		{
			pageViewController.View.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20.0f),
			pageViewController.View.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20.0f),
			View.SafeAreaLayoutGuide.BottomAnchor.ConstraintEqualTo(pageViewController.View.BottomAnchor, 20.0f),
			View.TrailingAnchor.ConstraintEqualTo(pageViewController.View.TrailingAnchor, 20.0f)
		});
		
		UIButton refreshButton = new UIButton(UIButtonType.System);
		refreshButton.SetTitle("Next", UIControlState.Normal);
		refreshButton.TintColor = UIColor.White;
		refreshButton.TranslatesAutoresizingMaskIntoConstraints = false;
		refreshButton.TouchUpInside += RefreshButton_OnTouchUpInside;
		refreshButton.SizeToFit();
		View.AddSubview(refreshButton);
		
		NSLayoutConstraint.ActivateConstraints(new []
		{
			refreshButton.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 20.0f),
			View.SafeAreaLayoutGuide.BottomAnchor.ConstraintEqualTo(refreshButton.BottomAnchor, 20.0f),
		});
		
		IEnumerable<SomeItem> someItems = await _service.GetSomeItemsAsync();
		_dataSource = new DataSource(someItems);
		_ = NextPage();
	}

	private Task NextPage()
	{
		UIViewController next = _dataSource.GetNextViewController();
		return _pageViewController.SetViewControllersAsync(new [] { next }, UIPageViewControllerNavigationDirection.Forward, false);
	}

	private void RefreshButton_OnTouchUpInside(object? sender, EventArgs e)
	{
		_ = NextPage();
	}
	
	private class DataSource
	{
		private readonly List<List<SomeItem>> _chunks;
		private int _currentPage;
		
		public DataSource(IEnumerable<SomeItem> items)
		{
			_currentPage = -1;
			
			List<List<SomeItem>>? someItemChunks = null;
		
			if (items.Take(1).TrySlice(6, out IEnumerable<IEnumerable<SomeItem>>? chunks))
			{
				someItemChunks = chunks?.Select(e => e.ToList()).ToList();
			}

			_chunks = someItemChunks ?? new List<List<SomeItem>>();
		}

		public UIViewController GetNextViewController()
		{
			if (!_chunks.Any())
			{
				return new UIViewController();
			}
			
			if (_currentPage < 0)
			{
				_currentPage = 0;
				return new SomeItemsViewController2(_chunks[_currentPage]);
			}
			
			int nextIndex = _currentPage + 1;
			int total = _chunks.Count;

			if (nextIndex == total)
			{
				_currentPage = 0;
				return new SomeItemsViewController2(_chunks[_currentPage]);
			}

			if (total < nextIndex)
			{
				return new UIViewController();
			}
			
			_currentPage = nextIndex;
			return new SomeItemsViewController2(_chunks[_currentPage]);
		}
	}
}