namespace TRViS;

public delegate void ValueChangedEventHandler<TValue>(
	object? sender,
	TValue oldValue,
	TValue newValue
);
