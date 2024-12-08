public class SystemGenerator : GenericSingletonClass<SystemGenerator>
{
    SunGenerator _sunGenerator;
    PlanetGenerator _planetGenerator;

    public override void Awake()
    {
        base.Awake();
        _sunGenerator = GetComponent<SunGenerator>();
        _planetGenerator = GetComponent<PlanetGenerator>();
    }

    void Start() => Generate();

    async void Generate()
    {
        await _sunGenerator.GenerateSuns();
        await _planetGenerator.GeneratePlanets();
    }
}
