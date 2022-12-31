using UniRx;

public class SenderProperty<T>
{
    public IReadOnlyReactiveProperty<T> sender => data;
    private ReactiveProperty<T> data;
    public bool isPause = false;

    public T Value => data.Value;

    public SenderProperty(T initialValue)
    {
        data = new ReactiveProperty<T>(initialValue);
    }

    public SenderProperty()
    {
        data = new ReactiveProperty<T>();
    }

    public void Send(T value)
    {
        if (isPause)
            return;

        data.Value = value;
    }
}