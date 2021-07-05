namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class SearchLaserBallFlicker : SineFlicker
    {
        public override float baseValue => 1.2f;

        public override float amplitude => .3f;

        public override float frequency => 3f;
    }
}