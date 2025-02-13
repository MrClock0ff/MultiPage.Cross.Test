using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using AndroidX.ViewPager2.Widget;
using MultiPage.Cross.Test;
using Color = Android.Graphics.Color;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using Orientation = Android.Widget.Orientation;

namespace MultiPage.Cross.Android.Test;

[Register(nameof(SomeItemsFragmentDialog))]
public class SomeItemsFragmentDialog : DialogFragment
{
	private readonly SomeItemService _someItemService;
	private readonly DataSource _dataSource;
	private ViewPager2? _viewPager;

	public SomeItemsFragmentDialog()
	{
		_someItemService = new SomeItemService();
		_dataSource = new DataSource();
	}
	
	public override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		SetStyle(StyleNoTitle, global::Android.Resource.Style.ThemeBlackNoTitleBarFullScreen);
	}

	public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
	{
		base.OnCreateView(inflater, container, savedInstanceState);

		LinearLayout innerContainer = new LinearLayout(Context)
		{
			Background = new ColorDrawable(Color.Yellow),
			Orientation = Orientation.Vertical
		};

		ViewPager2 viewPager2 = _viewPager = new ViewPager2(Context!)
		{
			LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
			Background = new ColorDrawable(Color.White)
		};
		
		viewPager2.RegisterOnPageChangeCallback(new InternalOnPageChangeCallback(() => viewPager2.Post(ResizeViewPager)));
		innerContainer.AddView(viewPager2);

		Button refreshButton = new Button(Context)
		{
			LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
			Gravity = GravityFlags.Left,
			Text = "Refresh"
		};

		innerContainer.AddView(refreshButton);

		RelativeLayout outerContainer = new RelativeLayout(Context)
		{
			Background = new ColorDrawable(Color.Red),
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
		};
		RelativeLayout.LayoutParams innerContainerLayoutParams =
			new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
		innerContainerLayoutParams.AddRule(LayoutRules.CenterInParent);
		outerContainer.AddView(innerContainer, innerContainerLayoutParams);
		return outerContainer;
	}

	public override async void OnViewCreated(View view, Bundle? savedInstanceState)
	{
		base.OnViewCreated(view, savedInstanceState);
		
		ViewPager2? viewPager = _viewPager;

		if (viewPager == null)
		{
			return;
		}

		IEnumerable<SomeItem> someItems = await Task.Run(() => _someItemService.GetSomeItemsAsync());
		_dataSource.Reload(someItems);
		viewPager.Adapter = new SomeItemsPageAdapter(this, _dataSource);
	}

	public override Dialog OnCreateDialog(Bundle? savedInstanceState)
	{
		Dialog dialog = base.OnCreateDialog(savedInstanceState);
		dialog.Window?.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		dialog.Window?.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
		dialog.Window?.SetDimAmount(0);
		return dialog;
	}

	public override void OnConfigurationChanged(Configuration newConfig)
	{
		base.OnConfigurationChanged(newConfig);
		_viewPager?.Post(ResizeViewPager);
	}

	private void ResizeViewPager()
	{
		ViewPager2? viewPager = _viewPager;
		ViewGroup.LayoutParams? viewPagerLayoutParams = viewPager?.LayoutParameters;

		if (viewPager == null || viewPagerLayoutParams == null)
		{
			return;
		}

		View? view = ChildFragmentManager.Fragments.ElementAtOrDefault(viewPager.CurrentItem)?.View;

		if (view == null)
		{
			return;
		}

		int widthSpec = View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified);
		int heightSpec = View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified);
		view.Measure(widthSpec, heightSpec);
		int viewWidth = view.MeasuredWidth;
		int viewHeight = view.MeasuredHeight;

		if (viewPagerLayoutParams.Width != viewWidth || viewPagerLayoutParams.Height != viewHeight)
		{
			viewPagerLayoutParams.Width = viewWidth;
			viewPagerLayoutParams.Height = viewHeight;
			viewPager.LayoutParameters = viewPagerLayoutParams;
		}
	}

	private class InternalOnPageChangeCallback : ViewPager2.OnPageChangeCallback
	{
		private readonly Action _action;
		
		public InternalOnPageChangeCallback(Action action)
		{
			_action = action;
		}

		public override void OnPageSelected(int position)
		{
			base.OnPageSelected(position);
			_action.Invoke();
		}
	}
}