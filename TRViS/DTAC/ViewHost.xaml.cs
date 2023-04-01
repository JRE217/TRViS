using System.ComponentModel;
using TRViS.ViewModels;

namespace TRViS.DTAC;

public partial class ViewHost : ContentPage
{
	static public readonly GridLength TitleViewHeight = new(45, GridUnitType.Absolute);

	DTACViewHostViewModel ViewModel { get; }

	public ViewHost(AppViewModel vm, EasterEggPageViewModel eevm)
	{
		Shell.SetNavBarIsVisible(this, false);

		InitializeComponent();

		TitleLabel.Text = vm.SelectedWork?.Name;
		TitleLabel.TextColor = MenuButton.TextColor = eevm.ShellTitleTextColor;

		TitleBGBoxView.BackgroundColor = eevm.ShellBackgroundColor;

		vm.PropertyChanged += Vm_PropertyChanged;
		eevm.PropertyChanged += Eevm_PropertyChanged;

		ViewModel = new(vm);
		BindingContext = ViewModel;

		ViewModel.PropertyChanged += ViewModel_PropertyChanged;

		VerticalStylePageView.SetBinding(VerticalStylePage.SelectedTrainDataProperty, new Binding()
		{
			Source = vm,
			Path = nameof(AppViewModel.SelectedTrainData)
		});

		VerticalStylePageRemarksView.SetBinding(WithRemarksView.RemarksDataProperty, new Binding()
		{
			Source = vm,
			Path = nameof(AppViewModel.SelectedTrainData)
		});

		UpdateContent();
	}

	private void MenuButton_Clicked(object? sender, EventArgs e)
	{
		Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
	}

	private void Eevm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender is not EasterEggPageViewModel vm)
			return;

		switch (e.PropertyName)
		{
			case nameof(EasterEggPageViewModel.ShellBackgroundColor):
				TitleBGBoxView.Color = vm.ShellBackgroundColor;
				break;

			case nameof(EasterEggPageViewModel.ShellTitleTextColor):
				TitleLabel.TextColor = MenuButton.TextColor = vm.ShellTitleTextColor;
				break;
		}
	}

	private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(AppViewModel.SelectedWork))
			TitleLabel.Text = (sender as AppViewModel)?.SelectedWork?.Name;
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(DTACViewHostViewModel.TabMode))
			UpdateContent();
	}

	void UpdateContent()
	{
		HakoView.IsVisible = ViewModel.IsHakoMode;
		VerticalStylePageRemarksView.IsVisible = ViewModel.IsVerticalViewMode;
		WorkAffixView.IsVisible = ViewModel.IsWorkAffixMode;
	}
}
