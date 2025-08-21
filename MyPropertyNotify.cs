using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeaponDevelopment;

public interface IPreciseNotifyPropertyChanged : INotifyPropertyChanged
{
    void SubscribePropertyChanged(string propertyName, Action handler);
    void UnsubscribePropertyChanged(string propertyName, Action handler);
}

public partial class MyPropertyNotify : IPreciseNotifyPropertyChanged
{
    private Dictionary<string, Action> PerPropertyHandlers { get; } = [];

    public void SubscribePropertyChanged(string propertyName, Action handler)
    {
        if (PerPropertyHandlers.TryGetValue(propertyName, out var existing))
        {
            PerPropertyHandlers[propertyName] = handler + existing;
        }
        else PerPropertyHandlers[propertyName] = handler;
    }

    public void UnsubscribePropertyChanged(string propertyName, Action handler)
    {
        if (PerPropertyHandlers.TryGetValue(propertyName, out var existing))
        {
            var newHandler = existing - handler;
            if (newHandler is null) PerPropertyHandlers.Remove(propertyName);
            else PerPropertyHandlers[propertyName] = newHandler;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if (PerPropertyHandlers.TryGetValue(propertyName, out var action)) action?.Invoke();
    }

    public partial string Name { get; set; }
    public partial string Name
    {
        get;
        set => SetProperty(ref field, value);
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class NotifyPropertyAttribute : Attribute;