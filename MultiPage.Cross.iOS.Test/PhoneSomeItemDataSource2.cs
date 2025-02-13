namespace MultiPage.Cross.Test
{
	/// <summary>
	/// Event Message collection view data source and delegate implementation which handles iPhone device edge cases.
	/// </summary>
	public class PhoneSomeItemDataSource2 : SomeItemDataSource2
	{
		public PhoneSomeItemDataSource2(IEnumerable<SomeItem> source) : base(source)
		{
		}

		/// <summary>
		/// Get .Zero insets when UI is in landscape orientation.
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="layout"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		[Export("collectionView:layout:insetForSectionAtIndex:")]
		public new UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout,
			nint section)
		{
			if (CheckIsPortrait())
			{
				return base.GetInsetForSection(collectionView, layout, section);
			}

			switch (section)
			{
				case 0:
					// Add some gap between this and bottom row to spread cells a little bit more
					return new UIEdgeInsets(0.0f, 0.0f, MidRowSpacing / 2.0f, 0.0f);
				case 1:
					// Add some gap between this and top row to spread cells a little bit more
					return new UIEdgeInsets(MidRowSpacing / 2.0f, 0.0f, 0.0f, 0.0f);
				default:
					// Should never get this but just in case (we should always have max 2 rows)
					return UIEdgeInsets.Zero;
			};
		}

		/// <summary>
		/// Spread cell equally when UI orientation is in ladscape.
		/// </summary>
		/// <param name="collectionView"></param>
		/// <param name="layout"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		[Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
		public new nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView,
			UICollectionViewLayout layout,
			nint section)
		{
			if (CheckIsPortrait())
			{
				return base.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
			}
			
			return MidRowSpacing;
		}

		/// <summary>
		/// Get size for collection view which will fit inside provided size.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public override CGSize SizeThatFits(CGSize size)
		{
			if (CheckIsPortrait())
			{
				return base.SizeThatFits(size);
			}

			CGSize cellSize = GetCellSizeForContainerSize(size);
			// 3 columns, 2 rows
			CGSize collectionSize = new CGSize(cellSize.Width * 3.0f, cellSize.Height * 2.0f);
			// Add column spacing to total width
			collectionSize.Width += MidRowSpacing * 2.0f;
			// Add row spacing to total height
			collectionSize.Height += MidRowSpacing;
			return collectionSize;
		}

		/// <summary>
		/// Split items based on UI orientation into:
		/// - Portrait: 4 sections where first and last sections can only have 1 item, middle max up to 2 items
		/// - Landscape: 2 sections, max up to 3 items per section
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		protected override List<List<SomeItem>> SplitIntoSections(IEnumerable<SomeItem> items)
		{
			if (CheckIsPortrait())
			{
				return base.SplitIntoSections(items);
			}

			// In landscape there will be only 2 sections
			List<SomeItem> allItems = items.Take(6).ToList();

			if (allItems.Count == 0)
			{
				return new List<List<SomeItem>>();
			}

			if (allItems.Count == 1)
			{
				return new List<List<SomeItem>> { new List<SomeItem> { allItems[0] } };
			}

			return allItems.Slice(3).Select(l => l.ToList()).ToList();
		}

		/// <summary>
		/// Get view cell size based on specified container size.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		protected override CGSize GetCellSizeForContainerSize(CGSize size)
		{
			if (CheckIsPortrait())
			{
				return base.GetCellSizeForContainerSize(size);
			}

			// For landscape orientation show only max up to 2 rows
			float maxHeight = (float)size.Height - MidRowSpacing;
			float cellSize = maxHeight / 2.0f;
			return new CGSize(Math.Floor(cellSize), Math.Floor(cellSize));
		}

		/// <summary>
		/// Check if device UI is in portrait orientation.
		/// </summary>
		/// <returns></returns>
		private bool CheckIsPortrait()
		{
			return UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait;
		}
	}
}