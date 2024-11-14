namespace MultiPage.Cross.Test;

public class SomeItemsViewController : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
{
	private readonly List<List<SomeItem>> _sections;
	private readonly UIColor[][] _colors;

	public SomeItemsViewController(IEnumerable<SomeItem> items)
	{
		
		_sections = SplitIntoSections(items);
		_colors = new UIColor[][]
		{
			new UIColor[] { UIColor.Blue },
			new UIColor[] { UIColor.Gray, UIColor.Red },
			new UIColor[] { UIColor.Yellow, UIColor.Green },
			new UIColor[] { UIColor.Purple }
		};
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		UICollectionViewFlowLayout collectionViewLayout = new UICollectionViewFlowLayout
		{
			ScrollDirection = UICollectionViewScrollDirection.Vertical,
			MinimumInteritemSpacing = 0.0f,
			MinimumLineSpacing = 0.0f,
		};
		UICollectionView collectionView = new UICollectionView(CGRect.Empty, collectionViewLayout)
		{
			TranslatesAutoresizingMaskIntoConstraints = false,
			ShowsHorizontalScrollIndicator = false,
			ShowsVerticalScrollIndicator = false,
			BackgroundColor = UIColor.Clear,
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

	[Export("numberOfSectionsInCollectionView:")]
	public nint NumberOfSections(UICollectionView collectionView)
	{
		return _sections.Count;
	}

	[Export("collectionView:numberOfItemsInSection:")]
	public nint GetItemsCount(UICollectionView collectionView, nint section)
	{
		return _sections[(int)section].Count;
	}

	[Export("collectionView:cellForItemAtIndexPath:")]
	public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
	{
		SomeItemCollectionViewCell cell = (SomeItemCollectionViewCell)collectionView.DequeueReusableCell(SomeItemCollectionViewCell.GetReuseIdentifier(), indexPath);
		cell.BackgroundColor = _colors[indexPath.Section][indexPath.Item];
		return cell;
	}

	[Export("collectionView:layout:sizeForItemAtIndexPath:")]
	public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
	{
		// 3 columns, max 4 rows
		return new CGSize(collectionView.Bounds.Size.Width / 3.0f, collectionView.Bounds.Size.Height / 4.0f);
	}

	[Export("collectionView:layout:insetForSectionAtIndex:")]
	public UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
	{
		if (section == 0 || section == NumberOfSections(collectionView) - 1)
		{
			// Left and Right insets size of one cell so cell appears centered
			return new UIEdgeInsets(0.0f, collectionView.Bounds.Size.Width / 3.0f, 0.0f, collectionView.Bounds.Size.Width / 3.0f);
		}

		// Cells align to left
		return UIEdgeInsets.Zero;
	}

	[Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
	public nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout,
		nint section)
	{
		return collectionView.Bounds.Size.Width / 3.0f;
	}

	/// <summary>
	/// Split items into 4 sections where first and last sections can only have 1 item, middle max up to 2 items.
	/// </summary>
	/// <param name="items"></param>
	/// <returns></returns>
	private List<List<SomeItem>> SplitIntoSections(IEnumerable<SomeItem> items)
	{
		List<SomeItem> allItems = items.Take(6).ToList();

		if (allItems.Count == 0)
		{
			return new List<List<SomeItem>>();
		}

		if (allItems.Count == 1)
		{
			return new List<List<SomeItem>> { new List<SomeItem> { allItems[0] } };
		}

		List<SomeItem> firstSection = new List<SomeItem> { allItems.First() };
		allItems.RemoveAt(0);
		
		List<SomeItem> lastSection = new List<SomeItem>();

		if (allItems.Count % 2 != 0)
		{
			lastSection.Add(allItems.Last());
			allItems.RemoveAt(allItems.Count - 1);
		}
		
		List<List<SomeItem>> middleSections = allItems.Slice(2)
			.Select(chunk => chunk.ToList())
			.ToList();

		List<List<SomeItem>> sections = new List<List<SomeItem>>();
		sections.Add(firstSection);
		sections.AddRange(middleSections);
		sections.Add(lastSection);

		return sections;
	}
}