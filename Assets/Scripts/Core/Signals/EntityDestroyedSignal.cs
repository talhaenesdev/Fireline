namespace FireLine.Scripts.Core.Signals
{
    public class EntityDestroyedSignal
    {
        public object Entity { get; }

        public EntityDestroyedSignal(object entity)
        {
            Entity = entity;
        }
    }
}