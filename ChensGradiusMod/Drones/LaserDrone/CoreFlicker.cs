namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class CoreFlicker : SineFlicker
    {
        public override float baseValue => 6f;
        public override float amplitude => 1f;
        public override float frequency => 4f;
    }
}