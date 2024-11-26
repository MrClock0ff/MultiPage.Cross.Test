using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using MultiPage.Cross.Test;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace MultiPage.Cross.Android.Test;

[Register(nameof(SomeItemsFragmentDialog))]
public class SomeItemsFragmentDialog : DialogFragment
{
	private readonly SomeItemService _someItemService;
	private readonly DataSource _dataSource;
	private ViewPager2 _viewPager;

	public SomeItemsFragmentDialog()
	{
		_someItemService = new SomeItemService();
		_dataSource = new DataSource();
	}
	
	public override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		SetStyle(StyleNoTitle, Theme);
	}

	public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
	{
		float maxWidth = Resources.DisplayMetrics?.WidthPixels ?? 1.0f;
		int innerContainerWidth = (int)(maxWidth * 0.7f);
		LinearLayout innerContainer = new LinearLayout(Context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(innerContainerWidth, ViewGroup.LayoutParams.WrapContent),
			Background = new ColorDrawable(Color.Black),
			Orientation = Orientation.Vertical
		};

		ViewPager2 viewPager2 = _viewPager = new ViewPager2(Context!)
		{
			LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
			Adapter = new SomeItemsPageAdapter(this, _dataSource)
		};

		innerContainer.AddView(viewPager2);

		Button refreshButton = new Button(Context)
		{
			LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
			Gravity = GravityFlags.Left,
			Text = "Refresh"
		};

		innerContainer.AddView(refreshButton);

		RelativeLayout outerContainer = new RelativeLayout(Context);
		outerContainer.AddView(innerContainer);
		return outerContainer;
	}

	public override async void OnViewCreated(View view, Bundle? savedInstanceState)
	{
		base.OnViewCreated(view, savedInstanceState);

		IEnumerable<SomeItem> someItems = await Task.Run(() => _someItemService.GetSomeItemsAsync());
		_dataSource.Reload(someItems);
		_viewPager.Adapter = new SomeItemsPageAdapter(this, _dataSource);
	}

	public override Dialog OnCreateDialog(Bundle? savedInstanceState)
	{
		Dialog dialog = base.OnCreateDialog(savedInstanceState);
		dialog.Window?.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		dialog.Window?.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
		dialog.Window?.SetDimAmount(0);
		return dialog;
	}
}