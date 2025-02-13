using ObjCRuntime;

namespace MultiPage.Cross.Test
{
	/// <summary>
	/// View cell representing single Event button.
	/// </summary>
	public class SomeItemCollectionViewCell2 : UICollectionViewCell
	{
		private SomeItem _eventMessage;
		private UIImageView _imageView;
		private bool _disposed;

		/// <summary>
		/// Statically resolve view cell reuse identifier.
		/// </summary>
		/// <returns></returns>
		public static string GetReuseIdentifier() => nameof(SomeItemCollectionViewCell2);

		/// <summary>
		/// View cell reuse identifier.
		/// </summary>
		public override NSString ReuseIdentifier
		{
			get
			{
				return new NSString(GetReuseIdentifier());
			}
		}

		/// <summary>
		/// Get or set event for this view cell.
		/// </summary>
		public SomeItem EventMessage
		{
			get
			{
				return _eventMessage;
			}

			set
			{
				if (_eventMessage == value)
				{
					return;
				}
				
				_eventMessage = value;
				OnEventMessageChanged();
			}
		}

		protected SomeItemCollectionViewCell2(NativeHandle handle) : base(handle)
		{
			SetupView();
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_imageView?.Dispose();
				_imageView = null;
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Setup view layout.
		/// </summary>
		private void SetupView()
		{
			BackgroundColor = UIColor.Clear;
			_imageView = new UIImageView
			{
				TintColor = UIColor.White,
				ContentMode = UIViewContentMode.ScaleAspectFit,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			Add(_imageView);

			NSLayoutConstraint.ActivateConstraints(new[]
			{
				_imageView.TopAnchor.ConstraintEqualTo(TopAnchor),
				BottomAnchor.ConstraintEqualTo(_imageView.BottomAnchor),
				_imageView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
				TrailingAnchor.ConstraintEqualTo(_imageView.TrailingAnchor),
			});
		}

		/// <summary>
		/// Update image view with new icon.
		/// </summary>
		private void OnEventMessageChanged()
		{
			_imageView.Image = GetImage();
		}

		/// <summary>
		/// Instantiate an image from specified resource path.
		/// </summary>
		/// <returns></returns>
		private UIImage GetImage()
		{
			try
			{
				if (string.IsNullOrEmpty(_eventMessage?.Icon))
				{
					return null;
				}

				UIImage image = UIImage.FromBundle(_eventMessage.Icon)
					?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
				return image;
			}
			catch (Exception e)
			{
				Console.WriteLine($"{nameof(SomeItemCollectionViewCell2)}:{nameof(GetImage)}: error: {e.Message}");
				return null;
			}
		}
	}
}