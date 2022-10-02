namespace MobRoulette.Core.Interfaces
{
    public interface IPooled : IReusable
    {
        int PrefabId { get; set; }
        bool IsInUse { get; set; }
        void OnCleanUp();
        void OnDestroy();
        void Init();
    }
}