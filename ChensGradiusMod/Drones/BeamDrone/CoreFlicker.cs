namespace Chen.GradiusMod.Drones.BeamDrone
{
    internal class CoreFlicker : SineFlicker
    {
        public float _baseValue = .45f;
        public float _amplitude = .03f;
        public float _frequency = 4f;

        public override float baseValue => _baseValue;
        public override float amplitude => _amplitude;
        public override float frequency => _frequency;
    }
}