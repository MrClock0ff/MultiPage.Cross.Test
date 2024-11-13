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
		if (_pageViewController != null)
		{
			IEnumerable<SomeItem> someItems = await _service.GetSomeItemsAsync();
			SomeItemDataSource dataSource = new SomeItemDataSource(someItems);
			_pageViewController.WeakDataSource = dataSource;
			_pageViewController.WeakDelegate = dataSource;
		}
	}
}