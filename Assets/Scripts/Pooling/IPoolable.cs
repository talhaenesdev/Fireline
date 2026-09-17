namespace FireLine.Scripts.Pooling
{
    public interface IPoolable
    {
        void Initialize(
            IPoolService poolService,
            string poolKey);

        void OnSpawn();

        void OnDespawn();
    }
}