public class HZBSConfigs
{
    public class Zombie
    {
        public string Name { get; set; } = string.Empty;
        public bool Enable { get; set; } = true;
        public float Volume { get; set; } = 1.0f;
        public string HurtSound { get; set; } = string.Empty;
        public string DieSound { get; set; } = string.Empty;
        public string PainSound { get; set; } = string.Empty;
        public string IdleSound { get; set; } = string.Empty;
        public float IdleInterval { get; set; } = 20.0f;
        public string BurnSound { get; set; } = string.Empty;
        public string ExplodeSound { get; set; } = string.Empty;
        public string HitSound { get; set; } = string.Empty;
        public string HitWallSound { get; set; } = string.Empty;
        public string SwingSound { get; set; } = string.Empty;
        public string PrecacheSounds { get; set; } = string.Empty;

    }
    public List<Zombie> ZombieList { get; set; } = new List<Zombie>();

}