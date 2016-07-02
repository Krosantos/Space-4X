public enum Terrain
{
    Space,
    AsteroidS,
    AsteroidM,
    AsteroidL,
    AsteroidX,
    Star,
    Planet,
    Blackhole,
    Hyperspace,
    Deadspace,
    IonCloud,
    Station
}
public enum ResourceType
{
    Nothing,
    Hydrocarbons,
    Polymers,
    Aluminum,
    NuclearMaterial,
    Water,
    HeavyGas,
    DarkMatter,
    NeutronMass
}

public enum ShipScale
{
    Small,
    Medium,
    Large,
    Capital
}

public enum MapState
{
    None,
    Requested,
    Complete
}

public enum RegionType
{
    Unassigned,
    SolarSystem,
    Riches,
    Dead,
    Asteroids,
    Sneaky,
    Mixed,
    PirateMaybe
}

public enum SectorType
{
    Unassigned,
    SystemCenter,
    Asteroids,
    Planet,
    Deadspace,
    Clouds,
    Blend,
    Anomaly
}