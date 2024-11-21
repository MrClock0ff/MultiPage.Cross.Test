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
		
		pageViewController.View!.BackgroundColor = UIColor.Orange;
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
		
		List<List<SomeItem>>? someItemChunks = null;
		
		if (someItems.TrySlice(6, out IEnumerable<IEnumerable<SomeItem>>? chunks))
		{
			someItemChunks = chunks?.Select(e => e.ToList()).ToList();
		}

		if (someItemChunks?.Count <= 0)
		{
			return;
		}

		List<SomeItemsViewController> viewControllers = someItemChunks.Select(chunk => new SomeItemsViewController(chunk)).ToList();
		_dataSource = new DataSource(viewControllers);
		_ = NextPage();
	}

	private Task NextPage()
	{
		UIViewController next = _dataSource.GetNextViewController();
		return _pageViewController.SetViewControllersAsync(new [] { next }, UIPageViewControllerNavigationDirection.Forward, true);
	}

	private void RefreshButton_OnTouchUpInside(object? sender, EventArgs e)
	{
		_ = NextPage();
	}
	
	private class DataSource
	{
		private List<UIViewController> _controllers;
		private UIViewController? _current;
		
		public DataSource(IEnumerable<UIViewController> viewControllers)
		{
			_controllers = viewControllers.ToList() ?? new List<UIViewController>();
		}

		public UIViewController GetNextViewController()
		{
			if (_current == null)
			{
				return _current = _controllers[0];
			}
			
			int currentIndex = _controllers.IndexOf(_current);

			if (currentIndex < 0)
			{
				return _current = null;
			}
			
			int nextIndex = currentIndex + 1;
			int total = _controllers.Count;

			if (nextIndex == total)
			{
				return _current = _controllers.FirstOrDefault();
			}

			if (total < nextIndex)
			{
				return _current = null;
			}
			
			return _current = _controllers[nextIndex];
		}
	}
}