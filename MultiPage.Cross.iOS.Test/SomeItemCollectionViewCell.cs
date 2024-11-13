using ObjCRuntime;

namespace MultiPage.Cross.Test;

public class SomeItemCollectionViewCell : UICollectionViewCell
{
	public static string GetReuseIdentifier() => nameof(SomeItemCollectionViewCell);
	
	public override NSString ReuseIdentifier
	{
		get
		{
			return new NSString(GetReuseIdentifier()); 
		}
	}

	protected SomeItemCollectionViewCell(NativeHandle handle) : base(handle)
	{
		SetupView();
	}

	private void SetupView()
	{
		
	}
}