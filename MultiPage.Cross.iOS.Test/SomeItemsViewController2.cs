namespace MultiPage.Cross.Test
{
	/// <summary>
	/// Events Hotkeys collection view controller which presents event buttons as circular layout of 6 buttons.
	/// </summary>
	public class SomeItemsViewController2 : UIViewController
	{
		private readonly IEnumerable<SomeItem> _source;
		private UICollectionView _collectionView;
		private NSLayoutConstraint _collectionViewWidthConstraint;
		private NSLayoutConstraint _collectionViewHeightConstraint;
		private bool _disposed;

		public SomeItemsViewController2(IEnumerable<SomeItem> source)
		{
			_source = source;
		}

		/// <summary>
		/// Prepare collection view and insert into parent view.
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UICollectionViewFlowLayout collectionViewLayout = new UICollectionViewFlowLayout
			{
				ScrollDirection = UICollectionViewScrollDirection.Vertical,
				MinimumInteritemSpacing = 0.0f,
				MinimumLineSpacing = 0.0f,
			};
			UICollectionView collectionView = _collectionView = 
				new UICollectionView(CGRect.Empty, collectionViewLayout)
			{
				TranslatesAutoresizingMaskIntoConstraints = false,
				ShowsHorizontalScrollIndicator = false,
				ShowsVerticalScrollIndicator = false,
				BackgroundColor = UIColor.Clear,
				ScrollEnabled = false,
				Bounces = false
			};
			collectionView.RegisterClassForCell(typeof(SomeItemCollectionViewCell2),
				SomeItemCollectionViewCell2.GetReuseIdentifier());

			View!.Add(collectionView);

			_collectionViewWidthConstraint = collectionView.WidthAnchor.ConstraintEqualTo(View.Bounds.Width);
			_collectionViewHeightConstraint = collectionView.HeightAnchor.ConstraintEqualTo(View.Bounds.Height);

			NSLayoutConstraint.ActivateConstraints(new[]
			{
				// Anchor to top
				collectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
				// And centre horizontally
				collectionView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
				// Set sizing
				_collectionViewWidthConstraint,
				_collectionViewHeightConstraint
			});

			ReloadDataSource();
			UpdateCollectionViewConstraints();
		}

		/// <summary>
		/// Update collection view dimensions on parent view size change.
		/// </summary>
		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			UpdateCollectionViewConstraints();
		}

		public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize(toSize, coordinator);
			coordinator.AnimateAlongsideTransition(_ => { }, _ => {
				ReloadDataSource();
				UpdateCollectionViewConstraints();
			});
		}

		/// <summary>
		/// Dispose all used resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_collectionViewWidthConstraint?.Dispose();
				_collectionViewWidthConstraint = null;
				_collectionViewHeightConstraint?.Dispose();
				_collectionViewHeightConstraint = null;
				_collectionView?.Dispose();
				_collectionView = null;
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Update collection view sizing constraints so it's always match it's content size.
		/// </summary>
		private void UpdateCollectionViewConstraints()
		{
			CGSize collectionSize = GetCollectionViewSize();
			_collectionViewWidthConstraint.Constant = collectionSize.Width;
			_collectionViewHeightConstraint.Constant = collectionSize.Height;

		}

		/// <summary>
		/// Get estimated collection view size.
		/// </summary>
		/// <returns></returns>
		private CGSize GetCollectionViewSize()
		{
			if (_collectionView == null || _collectionView.WeakDataSource == null)
			{
				return View!.Bounds.Size;
			}

			return ((SomeItemDataSource2)_collectionView.WeakDataSource).SizeThatFits(View!.Bounds.Size);
		}

		/// <summary>
		/// Reload collection view data source and delegate.
		/// </summary>
		private void ReloadDataSource()
		{
			if (_collectionView == null)
			{
				return;
			}

			SomeItemDataSource2 dataSource2 = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad
				? new SomeItemDataSource2(_source)
				: new PhoneSomeItemDataSource2(_source);
			
			_collectionView.WeakDataSource = dataSource2;
			_collectionView.WeakDelegate = dataSource2;
		}
	}
}