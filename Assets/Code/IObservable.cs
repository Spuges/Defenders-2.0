using System;

public interface IObservable<TData>
{
    public void Subscribe(Action<TData> callback);
    public void Unsubscribe(Action<TData> callback);
}

public class Observable<T> : IObservable<T>
{
    private event Action<T> action_event;

    public void Invoke(T data) => action_event?.Invoke(data);

    public void Subscribe(Action<T> callback)
    {
        action_event += callback;
    }

    public void Unsubscribe(Action<T> callback)
    {
        action_event -= callback;
    }

    public void Clear()
    {
        action_event = null;
    }
}

public class Observable
{
    private event Action action_event;

    public void Invoke() => action_event?.Invoke();

    public void Subscribe(Action callback)
    {
        action_event += callback;
    }

    public void Unsubscribe(Action callback)
    {
        action_event -= callback;
    }

    public void Clear()
    {
        action_event = null;
    }
}