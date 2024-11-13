namespace MultiPage.Cross.Test;

public class SomeItem
{
	public SomeItem(Guid id, string text, string icon)
	{
		Id = id;
		Text = text;
		Icon = icon;
	}

	public Guid Id { get; }

	public string Text { get; }

	public string Icon { get; }
}