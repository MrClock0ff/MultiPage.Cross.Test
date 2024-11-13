namespace MultiPage.Cross.Test;

public class SomeItemsViewController : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
{
	private readonly IEnumerable<SomeItem> _items;

	public SomeItemsViewController(IEnumerable<SomeItem> items)
	{
		_items = items;
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		UICollectionViewFlowLayout collectionViewLayout = new UICollectionViewFlowLayout
		{
			ScrollDirection = UICollectionViewScrollDirection.Vertical,
			MinimumInteritemSpacing = 0.0f,
			MinimumLineSpacing = 1.0f,
		};
		UICollectionView collectionView = new UICollectionView(CGRect.Empty, collectionViewLayout)
		{
			TranslatesAutoresizingMaskIntoConstraints = false,
			ShowsHorizontalScrollIndicator = false,
			ShowsVerticalScrollIndicator = false,
			ScrollEnabled = false,
			DataSource = this,
			Delegate = this,
			Bounces = false
		};
		collectionView.RegisterClassForCell(typeof(SomeItemCollectionViewCell), SomeItemCollectionViewCell.GetReuseIdentifier());
		
		View!.Add(collectionView);
		
		NSLayoutConstraint.ActivateConstraints(new []
		{
			collectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
			collectionView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
			collectionView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
			collectionView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor)
		});
	}
	
	

	[Export("collectionView:numberOfItemsInSection:")]
	public nint GetItemsCount(UICollectionView collectionView, nint section)
	{
		return _items.Count();
	}

	[Export("collectionView:cellForItemAtIndexPath:")]
	public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
	{
		SomeItemCollectionViewCell cell = (SomeItemCollectionViewCell)collectionView.DequeueReusableCell(SomeItemCollectionViewCell.GetReuseIdentifier(), indexPath);
		cell.BackgroundColor = indexPath.Item % 2 == 0 ? UIColor.Red : UIColor.Blue;
		return cell;
	}

	[Export("collectionView:layout:sizeForItemAtIndexPath:")]
	public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
	{
		if (indexPath.Item == 0 || indexPath.Item == 5)
		{
			return new CGSize(collectionView.Bounds.Size.Width, collectionView.Bounds.Size.Height / 4.0f);
		}
		else
		{
			return new CGSize(collectionView.Bounds.Size.Width / 2.0f, collectionView.Bounds.Size.Height / 4.0f);
		}
	}
}