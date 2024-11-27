namespace MultiPage.Cross.Test
{
	/// <summary>
	/// Event Message collection view data source and delegate implementation.
	/// 
	/// This data source/delegate will lay items out as grid 3 x 4.
	/// </summary>
	public class SomeItemDataSource2 : NSObject, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
	{
		private readonly List<List<SomeItem>> _sections;
		
		public SomeItemDataSource2(IEnumerable<SomeItem> source)
		{
			_sections = SplitIntoSections(source);
			MidRowSpacing = 48.0f;
		}
		
		#region IUICollectionViewDataSource interface implementation
		
		/// <summary>
		/// Returns a number of sections which are used as rows in collection view (1 section per row).
		/// </summary>
		/// <param name="collectionView"></param>
		/// <returns></returns>
		[Export("numberOfSectionsInCollectionView:")]
		public nint NumberOfSections(UICollectionView collectionView)
		{
			return _sections.Count;
		}

		/// <summary>
		/// Returns number of items in each section (or each row).
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		[Export("collectionView:numberOfItemsInSection:")]
		public nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return _sections[(int)section].Count;
		}

		/// <summary>
		/// Prepare collection view cell and inject event model.
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="indexPath"></param>
		/// <returns></returns>
		[Export("collectionView:cellForItemAtIndexPath:")]
		public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			SomeItemCollectionViewCell2 cell2 =
				(SomeItemCollectionViewCell2)collectionView.DequeueReusableCell(
					SomeItemCollectionViewCell2.GetReuseIdentifier(), indexPath);

			try
			{
				List<UIColor> colors = new List<UIColor>
					{ UIColor.Blue, UIColor.Brown, UIColor.Cyan, UIColor.Purple, UIColor.Red, UIColor.Green };
				SomeItem eventMessage = _sections[indexPath.Section].ElementAt(indexPath.Row);
				cell2.EventMessage = eventMessage;
				cell2.BackgroundColor = colors[indexPath.Section + indexPath.Row]; 
			}
			catch (Exception e)
			{
				Console.WriteLine(
					$"{nameof(SomeItemsViewController2)}:{nameof(GetCell)}: Unable to populate view cell for index {indexPath.Section}:{indexPath.Item}. Error: {e.Message}");
			}

			return cell2;
		}
		
		#endregion

		#region interface IUICollectionViewDelegateFlowLayout interface implementation
		
		/// <summary>
		/// Get view cell sizing so collection view can fit 3 view cells per section (row).
		/// Makes it to fit into 3 x 4 grid.
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="layout"></param>
		/// <param name="indexPath"></param>
		/// <returns></returns>
		[Export("collectionView:layout:sizeForItemAtIndexPath:")]
		public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout,
			NSIndexPath indexPath)
		{
			CGSize cellSize = GetCellSizeForContainerView(collectionView);
			return cellSize;
		}

		/// <summary>
		/// First and last sections will have only 1 view cell centered in the middle of the section.
		/// To accomplish this it will fill the gaps with view cell equivalent spacing.
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="layout"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		[Export("collectionView:layout:insetForSectionAtIndex:")]
		public UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			switch (section)
			{
				case 0:
				case 3:
					// Outer rows
					// Left and Right insets size of one cell so cell appears centered
					CGSize cellSize = GetCellSizeForContainerView(collectionView);
					return new UIEdgeInsets(0.0f, cellSize.Width, 0.0f, cellSize.Width);
				case 1:
					// Middle top row
					// Add some gap between this and middle bottom row to spread cells a little bit more
					return new UIEdgeInsets(0.0f, 0.0f, MidRowSpacing / 2.0f, 0.0f);
				case 2:
					// Middle bottom row
					// Add some gap between this and middle top row to spread cells a little bit more
					return new UIEdgeInsets(MidRowSpacing / 2.0f, 0.0f, 0.0f, 0.0f);
				default:
					// Should never get this but just in case (we should always have max 4 rows)
					return UIEdgeInsets.Zero;
			}
		}

		/// <summary>
		/// Insert view cell size spacing between two view cells so it makes middle column appear clear for middle sections (rows).
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="layout"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		[Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
		public nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout,
			nint section)
		{
			CGSize cellSize = GetCellSizeForContainerView(collectionView);
			return cellSize.Width;
		}

		#endregion

		/// <summary>
		/// Get size for collection view which will fit inside provided size.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public virtual CGSize SizeThatFits(CGSize size)
		{
			CGSize cellSize = GetCellSizeForContainerSize(size);
			// 3 columns, 4 rows
			CGSize collectionSize = new CGSize(cellSize.Width * 3.0f, cellSize.Height * 4.0f);
			collectionSize.Height += MidRowSpacing;
			return collectionSize;
		}

		/// <summary>
		/// Spacing between 2nd and 3rd rows.
		/// </summary>
		protected float MidRowSpacing { get; }

		/// <summary>
		/// Split items into 4 sections where first and last sections can only have 1 item, middle max up to 2 items.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		protected virtual List<List<SomeItem>> SplitIntoSections(IEnumerable<SomeItem> items)
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

			List<List<SomeItem>> middleSections = allItems.Slice(2).Select(l => l.ToList())
				.Select(chunk => chunk)
				.ToList();

			List<List<SomeItem>> sections = new List<List<SomeItem>>();
			sections.Add(firstSection);
			sections.AddRange(middleSections);
			sections.Add(lastSection);

			return sections;
		}
		
		/// <summary>
		/// Get view cell size based on specified container size.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		protected virtual CGSize GetCellSizeForContainerSize(CGSize size)
		{
			float maxWidth = (float)size.Width;
			float maxHeight = (float)size.Height - MidRowSpacing;
			float ratio = maxHeight / maxWidth;
			float cellSize = ratio >= 4.0f / 3.0f
				? maxWidth / 3.0f
				: maxHeight / 4.0f;
			return new CGSize(Math.Floor(cellSize), Math.Floor(cellSize));
		}
		
		/// <summary>
		/// Get view cell size based on container view size.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		private CGSize GetCellSizeForContainerView(UIView view)
		{
			CGSize baseSize = view.Superview != null 
				? view.Superview.Bounds.Size 
				: view.Bounds.Size;
			return GetCellSizeForContainerSize(baseSize);
		}
	}
}