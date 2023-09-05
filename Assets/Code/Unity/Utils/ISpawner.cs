namespace SaloonSlingers.Unity
{
    public interface ISpawner<T>
    {
        public T Spawn();
        public void Despawn(T t);
    }
}
