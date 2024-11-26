using System.Collections;
using System.Diagnostics;
using System.Drawing;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.IO;
using MultiPage.Cross.Test;
using Color = Android.Graphics.Color;
using Fragment = AndroidX.Fragment.App.Fragment;
using GridLayout = AndroidX.GridLayout.Widget.GridLayout;
using JObject = Java.Lang.Object;
using JClass = Java.Lang.Class;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;

namespace MultiPage.Cross.Android.Test;

[Register(nameof(SomeItemsGridFragment))]
public class SomeItemsGridFragment : Fragment
{
	private const float DEFAULT_GRID_PADDING_DP = 15.0f;
	private readonly Color[] _cellColors = [Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange, Color.Purple];

	public SomeItemsGridFragment()
	{
	}

	public void SetSomeItems(IEnumerable<SomeItem>? items)
	{
		Bundle someItemsBundle = new Bundle();
		someItemsBundle.PutSerializable(nameof(SomeItemsSerializable), new SomeItemsSerializable(items ?? []));
		Arguments = someItemsBundle;
	}

	public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
	{
		GridLayout gridLayout = new GridLayout(Context)
		{
			Orientation = GridLayout.Vertical
		};
		
		int gridPadding = (int)GetDefaultGridPaddingPx();
		gridLayout.SetPadding(gridPadding, gridPadding, gridPadding, gridPadding);

		FrameLayout viewGroup = new FrameLayout(Context);
		viewGroup.AddView(gridLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
		return viewGroup;
	}

	public override void OnResume()
	{
		base.OnResume();
		ReloadGridLayout(Arguments);
	}

	private void ReloadGridLayout(Bundle? bundle)
	{
		ViewGroup? viewGroup = View as ViewGroup;
		GridLayout? gridLayout = viewGroup?.GetChildAt(0) as GridLayout;

		if (gridLayout == null)
		{
			return;
		}

		gridLayout.RemoveAllViews();

		GridLayoutManager layoutManager = new GridLayoutManager(GetLayoutMaxSize());
		Size gridSize = layoutManager.GetGridSize();
		gridLayout.ColumnCount = gridSize.Width;
		gridLayout.RowCount = gridSize.Height;
		
		SizeF cellSize = layoutManager.GetCellSize();
		List<SomeItem> items = GetItemsFromBundle(bundle);

		for(int itemIndex = 0; itemIndex < items.Count; itemIndex++)
		{
			SomeItem item = items[itemIndex];
			ImageButton button = new ImageButton(Context)
			{
				Background = new ColorDrawable(_cellColors[itemIndex])
			};
			button.Click += (s, e) => Debug.WriteLine(item.Text);

			Point cellPosition = layoutManager.GetItemPosition(itemIndex);
			GridLayout.LayoutParams buttonLayoutParams = new GridLayout.LayoutParams(new ViewGroup.LayoutParams((int)cellSize.Width, (int)cellSize.Height));
			buttonLayoutParams.RowSpec = GridLayout.InvokeSpec(cellPosition.Y, GridLayout.CenterAlignment);
			buttonLayoutParams.ColumnSpec = GridLayout.InvokeSpec(cellPosition.X, GridLayout.CenterAlignment);

			gridLayout.AddView(button, buttonLayoutParams);
		}
	}

	private SizeF GetLayoutMaxSize()
	{
		DisplayMetrics? metrics = Resources.DisplayMetrics;

		if (metrics == null)
		{
			return SizeF.Empty;
		}

		float maxWidth = metrics.WidthPixels;
		float maxHeight = metrics.HeightPixels;

		// Use only 70% of available screen
		maxWidth *= 0.7f;
		maxHeight *= 0.7f;

		// Left and right padding
		maxWidth -= GetDefaultGridPaddingPx() * 2f;
		// Top and bottom padding
		maxHeight -= GetDefaultGridPaddingPx() * 2f;

		maxWidth = maxWidth >= 0f ? maxWidth : 0f;
		maxHeight = maxHeight >= 0f ? maxHeight : 0f;
		return new SizeF(maxWidth, maxHeight);
	}

	private float GetDefaultGridPaddingPx()
	{
		float dpi = Resources.DisplayMetrics?.Density ?? 1.0f;
		return dpi * DEFAULT_GRID_PADDING_DP;
	}

	private List<SomeItem> GetItemsFromBundle(Bundle? bundle)
	{
		SomeItemsSerializable? serializable;

		if (OperatingSystem.IsAndroidVersionAtLeast(33))
		{
			serializable = Arguments?.GetSerializable(nameof(SomeItemsSerializable), JClass.FromType(typeof(SomeItemsSerializable))) as SomeItemsSerializable;
		}
		else
		{
			serializable = Arguments?.GetSerializable(nameof(SomeItemsSerializable)) as SomeItemsSerializable;
		}

		return serializable?.Items.Take(6).ToList() ?? new List<SomeItem>();
	}

	private class SomeItemsSerializable : JObject, ISerializable
	{
		public SomeItemsSerializable(IEnumerable<SomeItem> items)
		{
			Items = items;
		}

		public IEnumerable<SomeItem> Items { get; }
	}
}