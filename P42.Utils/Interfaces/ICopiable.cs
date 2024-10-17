namespace P42.Utils;

public interface ICopiable<T>
{
    void PropertiesFrom(T other);
}

public interface ICloneable<T>
{
    T Clone();
}
