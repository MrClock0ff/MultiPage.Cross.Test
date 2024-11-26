using System.Drawing;

namespace MultiPage.Cross.Android.Test;

public class GridLayoutManager
{
	private readonly SizeF _viewportSize;

	public GridLayoutManager(SizeF viewPortSize)
	{
		_viewportSize = viewPortSize;
	}

	public Size GetGridSize()
	{
		GridLayoutOrientation orientation = GetLayoutOrientation();

		switch (orientation)
		{
			case GridLayoutOrientation.Landscape:
				return new Size(3, 2);
			case GridLayoutOrientation.Portrait:
			default:
				return new Size(3, 4);
		}
	}

	public SizeF GetCellSize()
	{
		GridLayoutOrientation orientation = GetLayoutOrientation();
		float maxCellSize;

		switch (orientation)
		{
			case GridLayoutOrientation.Portrait:
				maxCellSize =  _viewportSize.Width / 3f;
				break;
			case GridLayoutOrientation.Landscape:
				maxCellSize = _viewportSize.Height / 2f;
				break;
			default:
				maxCellSize = Math.Min(_viewportSize.Width, _viewportSize.Height) / 4f;
				break;
		}
		
		return new SizeF(maxCellSize, maxCellSize);
	}

	public Point GetItemPosition(int itemIndex)
	{
		GridLayoutOrientation orientation = GetLayoutOrientation();

		switch (orientation)
		{
			case GridLayoutOrientation.Landscape:
				return GetItemPositionForLandscape(itemIndex);
			case GridLayoutOrientation.Portrait:
			default:
				return GetItemPositionForPortrait(itemIndex);
		}
	}

	/// <summary>
	/// Grid 3 x 4
	/// </summary>
	/// <param name="itemIndex"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	private Point GetItemPositionForPortrait(int itemIndex)
	{
		switch (itemIndex)
		{
			case 0:
				return new Point(1, 0);
			case 1:
				return new Point(0, 1);
			case 2:
				return new Point(2, 1);
			case 3:
				return new Point(0, 2);
			case 4:
				return new Point(2, 2);
			case 5:
				return new Point(1, 3);
			default:
				throw new IndexOutOfRangeException();
		}
	}

	/// <summary>
	/// Grid 3 x 2
	/// </summary>
	/// <param name="itemIndex"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	private Point GetItemPositionForLandscape(int itemIndex)
	{
		switch (itemIndex)
		{
			case 0:
				return new Point(0, 0);
			case 1:
				return new Point(1, 0);
			case 2:
				return new Point(2, 0);
			case 3:
				return new Point(0, 1);
			case 4:
				return new Point(1, 1);
			case 5:
				return new Point(2, 1);
			default:
				throw new IndexOutOfRangeException();
		}
	}

	private GridLayoutOrientation GetLayoutOrientation()
	{
		if (_viewportSize.Height / _viewportSize.Width >= 4f / 3f)
		{
			return GridLayoutOrientation.Portrait;
		}
		
		if (_viewportSize.Height / _viewportSize.Width <= 3f / 4f)
		{
			return GridLayoutOrientation.Landscape;
		}

		return GridLayoutOrientation.All;
	}

	private enum GridLayoutOrientation
	{
		All,
		Portrait,
		Landscape
	}
}