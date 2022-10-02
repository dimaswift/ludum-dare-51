using System.Collections.Generic;

namespace MobRoulette.Core.Domain
{
    public class PlayerSave
    {
        public int Coins { get; set; }
        public int MobsKilledRecord { get; set; }
        public int RecordWave { get; set; }
        public int MobsKilledTotal { get; set; }
        public Dictionary<string,bool> UnlockedGuns { get; set; }
    }
}