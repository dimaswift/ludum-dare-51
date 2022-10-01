namespace MobRoulette.Core.Interfaces
{
    public interface IPooled : IReusable
    {
        int PrefabId { get; set; }
        void CleanUp();
        void Init();
    }
}