namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class CoreFlicker : SineFlicker
    {
        public float _baseValue = .6f;
        public float _amplitude = .1f;
        public float _frequency = .4f;

        public override float baseValue => _baseValue;
        public override float amplitude => _amplitude;
        public override float frequency => _frequency;
    }
}