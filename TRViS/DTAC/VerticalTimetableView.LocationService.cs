using DependencyPropertyGenerator;

using TRViS.IO.Models;
using TRViS.Services;

namespace TRViS.DTAC;

[DependencyProperty<bool>("IsRunStarted")]
[DependencyProperty<bool>("IsLocationServiceEnabled")]
public partial class VerticalTimetableView : Grid
{
	LocationService LocationService { get; } = new();

	public event EventHandler<ValueChangedEventArgs<bool>>? IsLocationServiceEnabledChanged;
	partial void OnIsLocationServiceEnabledChanged(bool newValue)
	{
		if (newValue)
			SetNearbyCheckInfo(CurrentRunningRow);

		LocationService.IsEnabled = newValue;
		IsLocationServiceEnabledChanged?.Invoke(this, new(!newValue, newValue));
	}

	private void LocationService_IsNearbyChanged(object? sender, bool oldValue, bool newValue)
	{
		if (!IsRunStarted || !IsEnabled || CurrentRunningRow is null || !LocationService.IsEnabled)
			return;

		if (newValue)
		{
			SetCurrentRunningRow(NextRunningRow);
		}
		else if (CurrentRunningRow is not null)
		{
			CurrentRunningRow.LocationState = VerticalTimetableRow.LocationStates.RunningToNextStation;

			SetNearbyCheckInfo(NextRunningRow);
		}
	}

	private void SetNearbyCheckInfo(VerticalTimetableRow? nextRunningRow)
	{
		if (nextRunningRow?.RowData is TimetableRow nextRowData)
		{
			LocationService.NearbyCenter
				= nextRowData.Location is LocationInfo
				{
					Latitude_deg: double lat,
					Longitude_deg: double lon
				}
					? new Location(lat, lon)
					: null;

			LocationService.NearbyRadius_m = nextRowData.Location.OnStationDetectRadius_m ?? LocationService.DefaultNearbyRadius_m;
		}
	}

	public void SetCurrentRunningRow(int index)
		=> SetCurrentRunningRow(index, RowViewList.ElementAtOrDefault(index));

	public void SetCurrentRunningRow(VerticalTimetableRow? value)
		=> SetCurrentRunningRow(value is null ? -1 : RowViewList.IndexOf(value), value);

	void SetCurrentRunningRow(int index, VerticalTimetableRow? value)
	{
		if (CurrentRunningRowIndex == index || CurrentRunningRow == value)
			return;

		if (RowViewList.ElementAtOrDefault(index) != value)
			throw new ArgumentException("value is not match with element at given index", nameof(value));

		MainThread.BeginInvokeOnMainThread(() =>
		{
			if (_CurrentRunningRow is not null)
				_CurrentRunningRow.LocationState = VerticalTimetableRow.LocationStates.Undefined;

			_CurrentRunningRow = value;

			if (value is not null)
			{
				CurrentRunningRowIndex = index;
				UpdateCurrentRunningLocationVisualizer(value, VerticalTimetableRow.LocationStates.AroundThisStation);

				if (value.LocationState != VerticalTimetableRow.LocationStates.Undefined)
				{
					ScrollRequested?.Invoke(this, new(Math.Max(value.RowIndex - 1, 0) * RowHeight.Value));
				}
			}
			else
				CurrentRunningRowIndex = -1;

			NextRunningRow = RowViewList.ElementAtOrDefault(index + 1);
		});
	}

	void UpdateCurrentRunningLocationVisualizer(VerticalTimetableRow row, VerticalTimetableRow.LocationStates states)
	{
		row.LocationState = states;

		int rowCount = row.RowIndex;

		Grid.SetRow(CurrentLocationBoxView, rowCount);
		Grid.SetRow(CurrentLocationLine, rowCount);

		CurrentLocationBoxView.IsVisible = row.LocationState
			is VerticalTimetableRow.LocationStates.AroundThisStation
			or VerticalTimetableRow.LocationStates.RunningToNextStation;
		CurrentLocationLine.IsVisible = row.LocationState is VerticalTimetableRow.LocationStates.RunningToNextStation;

		CurrentLocationBoxView.Margin = row.LocationState
			is VerticalTimetableRow.LocationStates.RunningToNextStation
			? new(0, -(RowHeight.Value / 2)) : new(0);
	}
}
