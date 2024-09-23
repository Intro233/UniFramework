using System;

public class ObjectPool<T> : Pool<T>
{
    private readonly Action<T> recycleMethod;

    public ObjectPool(Func<T> creatMethod, Action<T> recycleMethod = null)
    {
        createFactory = new ObjectFactory<T>(creatMethod);
        this.recycleMethod = recycleMethod;
    }

    public override bool Recycle(T obj)
    {
        recycleMethod?.Invoke(obj);
        cacheStack.Push(obj);
        return true;
    }
}