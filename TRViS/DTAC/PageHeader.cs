using DependencyPropertyGenerator;

namespace TRViS.DTAC;

[DependencyProperty<bool>("IsOpen")]
[DependencyProperty<bool>("IsRunning")]
[DependencyProperty<bool>("IsLocationServiceEnabled")]
public partial class PageHeader : Grid
{
	static readonly ColumnDefinitionCollection DefaultColumnDefinitions = new()
	{
		new ColumnDefinition(new GridLength(1, GridUnitType.Star)),

		// under total: 378
		new ColumnDefinition(186),
		new ColumnDefinition(128),
		new ColumnDefinition(60),
	};

	#region Affect Date Label
	const string AffectDateLabelTextPrefix = "行路施行日\n";

	readonly Label AffectDateLabel = DTACElementStyles.LabelStyle<Label>();

	string _AffectDateLabelText = "";
	public string AffectDateLabelText
	{
		get => _AffectDateLabelText;
		set
		{
			if (_AffectDateLabelText == value)
				return;

			_AffectDateLabelText = value;

			AffectDateLabel.Text = AffectDateLabelTextPrefix + value;
		}
	}
	#endregion

	#region Start / End Run Button
	readonly StartEndRunButton StartEndRunButton = new();

	partial void OnIsRunningChanged(bool newValue)
	{
		StartEndRunButton.IsChecked = newValue;

		LocationServiceButton.IsEnabled = newValue;
	}

	private void StartEndRunButton_IsCheckedChanged(object? sender, ValueChangedEventArgs<bool> e)
	{
		this.IsRunning = e.NewValue;

		LocationServiceButton.IsEnabled = e.NewValue;
	}
	#endregion

	#region Location Service Button
	readonly LocationServiceButton LocationServiceButton = new();

	public event EventHandler<ValueChangedEventArgs<bool>>? IsLocationServiceEnabledChanged
	{
		add => LocationServiceButton.IsCheckedChanged += value;
		remove => LocationServiceButton.IsCheckedChanged -= value;
	}

	partial void OnIsLocationServiceEnabledChanged(bool newValue)
	{
		LocationServiceButton.IsChecked = newValue;
	}

	private void LocationServiceButton_IsCheckedChanged(object? sender, ValueChangedEventArgs<bool> e)
	{
		IsLocationServiceEnabled = e.NewValue;
	}
	#endregion

	#region Open / Close Button
	readonly OpenCloseButton OpenCloseButton = new();

	public event EventHandler<ValueChangedEventArgs<bool>>? IsOpenChanged
	{
		add => OpenCloseButton.IsOpenChanged += value;
		remove => OpenCloseButton.IsOpenChanged -= value;
	}

	partial void OnIsOpenChanged(bool newValue)
		=> OpenCloseButton.IsOpen = newValue;

	private void OpenCloseButton_IsOpenChanged(object? sender, ValueChangedEventArgs<bool> e)
	{
		IsOpen = e.NewValue;
	}
	#endregion

	public PageHeader()
	{
		ColumnDefinitions = DefaultColumnDefinitions;

		AffectDateLabel.Margin = new(18, 4);
		AffectDateLabel.FontSize = 18;
		AffectDateLabel.HorizontalOptions = LayoutOptions.Start;
		AffectDateLabel.Text = AffectDateLabelTextPrefix;

		StartEndRunButton.VerticalOptions = LayoutOptions.Center;
		StartEndRunButton.HorizontalOptions = LayoutOptions.End;
		StartEndRunButton.Margin = new(2);
		StartEndRunButton.IsCheckedChanged += (_, e) => this.IsRunning = e.NewValue;

		LocationServiceButton.IsEnabled = false;
		LocationServiceButton.Margin = new(4, 8);
		LocationServiceButton.IsCheckedChanged += LocationServiceButton_IsCheckedChanged;

		OpenCloseButton.TextWhenOpen = "\xe5ce";
		OpenCloseButton.TextWhenClosed = "\xe5cf";
		OpenCloseButton.IsOpenChanged += OpenCloseButton_IsOpenChanged;
		OpenCloseButton.HorizontalOptions = LayoutOptions.Center;
		OpenCloseButton.VerticalOptions = LayoutOptions.Center;

		this.Add(
			AffectDateLabel,
			column: 0
		);
		this.Add(StartEndRunButton,
			column: 1
		);
		this.Add(LocationServiceButton,
			column: 2
		);
		this.Add(
			OpenCloseButton,
			column: 3
		);
	}
}
