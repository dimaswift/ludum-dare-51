namespace MobRoulette.Core.Interfaces
{
    public interface IPooled
    {
        int PrefabId { get; set; }
        void Prepare();
        void CleanUp();
        void Init();
    }
}