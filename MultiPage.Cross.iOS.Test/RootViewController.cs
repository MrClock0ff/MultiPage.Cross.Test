using CoreFoundation;

namespace MultiPage.Cross.Test;

public class RootViewController : UIViewController
{
	private readonly SomeItemService _service;
	private UIPageViewController? _pageViewController;

	public RootViewController()
	{
		_service = new SomeItemService();
	}

	public override void ViewDidLoad()
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
		refreshButton.SetTitle("Refresh", UIControlState.Normal);
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
	}

	private async void RefreshButton_OnTouchUpInside(object? sender, EventArgs e)
	{
		if (_pageViewController == null)
		{
			return;
		}
		
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
		DataSource dataSource = new DataSource(viewControllers);
		_pageViewController.DataSource = dataSource;
		await _pageViewController.SetViewControllersAsync(new [] {viewControllers[0]}, UIPageViewControllerNavigationDirection.Forward, true);
	}
	
	private class DataSource : NSObject, IUIPageViewControllerDataSource
	{
		private List<UIViewController> _controllers;
		public DataSource(IEnumerable<UIViewController> viewControllers)
		{
			_controllers = viewControllers.ToList() ?? new List<UIViewController>();
		}

		[Export("pageViewController:viewControllerBeforeViewController:")]
		public UIViewController GetPreviousViewController(UIPageViewController pageViewController,
			UIViewController referenceViewController)
		{
			int currentIndex = _controllers.IndexOf(referenceViewController);
			
			if (currentIndex < 0)
			{
				return null;
			}
			
			int prevIndex = currentIndex - 1;

			if (prevIndex < 0)
			{
				return _controllers.LastOrDefault();
			}

			if (_controllers.Count < prevIndex)
			{
				return null;
			}
			
			return _controllers[prevIndex];
		}

		[Export("pageViewController:viewControllerAfterViewController:")]
		public UIViewController GetNextViewController(UIPageViewController pageViewController,
			UIViewController referenceViewController)
		{
			int currentIndex = _controllers.IndexOf(referenceViewController);

			if (currentIndex < 0)
			{
				return null;
			}
			
			int nextIndex = currentIndex + 1;
			int total = _controllers.Count;

			if (nextIndex == total)
			{
				return _controllers.FirstOrDefault();
			}

			if (total < nextIndex)
			{
				return null;
			}
			
			return _controllers[nextIndex];
		}
	}
}