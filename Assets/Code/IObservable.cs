using System;

public interface IObservable<TData>
{
    public void Subscribe(Action<TData> callback);
    public void Unsubscribe(Action<TData> callback);
}
