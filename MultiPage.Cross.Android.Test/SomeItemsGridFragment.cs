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
		int gridPadding = (int)GetDefaultGridPaddingPx();
		ViewGroup.MarginLayoutParams gridLayoutParams =
			new ViewGroup.MarginLayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
		gridLayoutParams.SetMargins(gridPadding, gridPadding, gridPadding, gridPadding);
		GridLayout gridLayout = new GridLayout(Context)
		{
			Orientation = GridLayout.Vertical,
			LayoutParameters = gridLayoutParams,
			Background = new ColorDrawable(Color.Beige)
		};

		FrameLayout viewGroup = new FrameLayout(Context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
			Background = new ColorDrawable(Color.Aqua)
		};
		viewGroup.AddView(gridLayout);

		ReloadGridLayout(gridLayout, Arguments);

		return viewGroup;
	}

	private void ReloadGridLayout(GridLayout gridLayout, Bundle? bundle)
	{
		gridLayout.RemoveAllViews();
		
		GridLayoutManager layoutManager = new GridLayoutManager(GetLayoutMaxSize());
		Size gridSize = layoutManager.GetGridSize();
		gridLayout.ColumnCount = gridSize.Width;
		gridLayout.RowCount = gridSize.Height;
		
		SizeF cellSize = layoutManager.GetCellSize();
		List<SomeItem> items = GetItemsFromBundle(bundle);
		List<List<FrameLayout>> viewHolders = new List<List<FrameLayout>>();

		for (int row = 0; row < gridSize.Height; row++)
		{
			List<FrameLayout> rowViewHolders = new List<FrameLayout>();
			viewHolders.Add(rowViewHolders);
			
			for (int col = 0; col < gridSize.Width; col++)
			{
				Rectangle viewHolderSpacing = GetCellSpacing(new Point(col, row), gridSize);
				FrameLayout viewHolder = new FrameLayout(Context);
				GridLayout.LayoutParams viewHolderLayoutParams = new GridLayout.LayoutParams(new ViewGroup.LayoutParams((int)cellSize.Width, (int)cellSize.Height));
				viewHolderLayoutParams.SetMargins(viewHolderSpacing.X, viewHolderSpacing.Y, viewHolderSpacing.Width, viewHolderSpacing.Height);
				viewHolderLayoutParams.RowSpec = GridLayout.InvokeSpec(row, GridLayout.CenterAlignment);
				viewHolderLayoutParams.ColumnSpec = GridLayout.InvokeSpec(col, GridLayout.CenterAlignment);
				gridLayout.AddView(viewHolder, viewHolderLayoutParams);
				rowViewHolders.Add(viewHolder);
			}
		}

		for(int itemIndex = 0; itemIndex < items.Count; itemIndex++)
		{
			SomeItem item = items[itemIndex];
			ImageButton button = new ImageButton(Context)
			{
				Background = new ColorDrawable(_cellColors[itemIndex]),
				LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
			};
			button.Click += (s, e) => Debug.WriteLine(item.Text);

			Point cellPosition = layoutManager.GetItemPosition(itemIndex);
			viewHolders[cellPosition.Y][cellPosition.X].AddView(button);
		}
	}

	public Rectangle GetCellSpacing(Point cellIndex, Size gridSize)
	{
		int spacing = (int)GetDefaultGridPaddingPx();
		// Top row has no spacing at the top
		int top = cellIndex.Y == 0 ? 0 : spacing;
		// Bottom row has no spacing at the bottom
		int bottom = cellIndex.Y == gridSize.Height - 1 ? 0 : spacing;
		// Left column has no left spacing
		int left = cellIndex.X == 0 ? 0 : spacing;
		// Right column has no right spacing
		int right = cellIndex.X == gridSize.Width - 1 ? 0 : spacing;
		return new Rectangle(left, top, right, bottom);
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