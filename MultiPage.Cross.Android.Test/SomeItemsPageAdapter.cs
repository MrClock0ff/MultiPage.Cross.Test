using AndroidX.ViewPager2.Adapter;
using Java.Lang;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace MultiPage.Cross.Android.Test;

public class SomeItemsPageAdapter : FragmentStateAdapter
{
	private readonly DataSource _dataSource;
	private FragmentManager _fragmentManager;
	private bool _disposed;

	public SomeItemsPageAdapter(Fragment fragment, DataSource dataSource)
		: base(fragment)
	{
		_dataSource = dataSource;
		_fragmentManager = fragment.ChildFragmentManager;
	}

	public override int ItemCount
	{
		get
		{
			return _dataSource.ItemCount;
		}
	}

	public override Fragment CreateFragment(int index)
	{
		ClassLoader? classLoader = ClassLoader.SystemClassLoader;
		
		if (classLoader == null)
		{
			throw new IllegalStateException("Class loader is null");
		}
		
		SomeItemsGridFragment? fragment = _fragmentManager?.FragmentFactory?.Instantiate(classLoader, nameof(SomeItemsGridFragment)) as SomeItemsGridFragment;

		if (fragment == null)
		{
			throw new IllegalStateException("Fragment is null");
		}
		
		fragment.SetSomeItems(_dataSource.GetItem(index));

		return fragment;
	}
}