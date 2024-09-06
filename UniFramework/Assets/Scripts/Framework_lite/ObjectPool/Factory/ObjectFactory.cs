using System;

public class ObjectFactory<T> : IFactory<T>
{
    protected Func<T> factoryMethod;

    public ObjectFactory(Func<T> factoryMethod)
    {
        this.factoryMethod = factoryMethod;
    }
    public T Create()
    {
        return factoryMethod();
    }
}