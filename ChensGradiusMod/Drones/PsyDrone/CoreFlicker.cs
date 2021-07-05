namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class CoreFlicker : SineFlicker
    {
        public override float baseValue => .75f;
        public override float amplitude => .06f;
        public override float frequency => .9f;
    }
}