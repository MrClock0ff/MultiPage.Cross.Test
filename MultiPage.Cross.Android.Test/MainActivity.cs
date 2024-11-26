using AndroidX.Fragment.App;

namespace MultiPage.Cross.Android.Test;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : FragmentActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Set our view from the "main" layout resource
		SetContentView(Resource.Layout.activity_main);
		
		Button? button = FindViewById<Button>(Resource.Id.button);

		if (button != null)
		{
			button.Click += Button_OnClick;
		}
	}

	private void Button_OnClick(object? sender, EventArgs e)
	{
		SomeItemsFragmentDialog dialog = new SomeItemsFragmentDialog();
		dialog.Show(SupportFragmentManager, nameof(SomeItemsFragmentDialog));
	}
}