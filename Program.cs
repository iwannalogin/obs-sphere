using System.Security.Cryptography;

Console.WriteLine("Obsidian Sphere Calculations");

var waterDensityKG_M3 = 1d * 1000d;
var obsidianDensityKG_M3 = 2.55d * 1000d;

var sfToOakBridgeLengthKM = 3d;
var orbRadius = sfToOakBridgeLengthKM * 0.75d * 1000d;
var orbArea = 4d * Math.PI * Math.Pow(orbRadius, 2d );
var orbSolidVolume = (4d/3d) * Math.PI * Math.Pow(orbRadius, 3d );
var orbSolidMassKG = orbSolidVolume * obsidianDensityKG_M3;

Console.WriteLine( $"The start of the SF↔Oakland Bridge to the center of the island is approx. {sfToOakBridgeLengthKM:0.##} KM" );
Console.WriteLine( $"The Orb Radius looks to be 75% of the distance." );
//Console.WriteLine( $"Leaving us with..." );
//Console.WriteLine( $"A radius" );
Console.WriteLine( $"So, Radius = {orbRadius:0.##} M, Area = {orbArea:0.##} M2, Solid Volume = {orbSolidVolume:0.##} M3, Solid Mass: {orbSolidMassKG:0.##} KG" );

//Buoyant force = Fluid Density (kg/m3) * Volume m3 * Gravity g (m/s2)

double[] estSubmergedDepths = {
    0.001d,
    1d,
    orbRadius * 0.02
};

var estSubmergedProps = estSubmergedDepths.Select<double, ( double DepthM, double VolumeM3, double AreaM2 )>(
    depth => {
    return new (
        depth,
        Math.PI * Math.Pow(depth, 2d) / 3d * (3d * orbRadius - depth),
        2d * Math.PI * orbRadius * depth
    );
}).ToArray();

var gravityM_S2 = 9.8;
var buoyantForcesN = estSubmergedProps.Select( props => {
    return waterDensityKG_M3 * props.VolumeM3 * gravityM_S2;
}).ToArray();

var sphereMassKG = buoyantForcesN.Select( forceN => forceN / gravityM_S2 );
var sphereSizing = sphereMassKG.Select<double, ( double RadiusM, double ThicknessM, double VolumeM3, double MassKG )>( massKG => {
    var lessMassKG = orbSolidMassKG - massKG;
    var lessVolumeM3 = lessMassKG / obsidianDensityKG_M3;
    var lessRadiusM = Math.Pow( lessVolumeM3 / (4d/3d) / Math.PI, 1d/3d );

    return (
        orbRadius,
        orbRadius - lessRadiusM,
        lessVolumeM3,
        lessMassKG
    );
}).ToArray();

var averageUser = 137d / 2.2046;
var sphereCosts = sphereSizing.Select( size => size.MassKG ).Select<double,( double Material, double Commercial)>( massKG => {
    return (
        massKG * 5d, //https://coincodex.com/article/36994/how-much-is-obsidian-worth/
        massKG * averageUser * 50d //Obsidian.MD
    );
}).ToArray();

Console.WriteLine("Assuming a few depths, these are the properties of the sphere:");
for( var i = 0; i < estSubmergedDepths.Length; i++ ) {
    var submergedProps = estSubmergedProps[i];
    var buoyantForceN = buoyantForcesN[i];
    var sphereSize = sphereSizing[i];
    var sphereCost = sphereCosts[i];
    Console.WriteLine($"Variant {i}");
    Console.WriteLine($"-- Submerged Depth: {submergedProps.DepthM:0.####} M");
    Console.WriteLine($"-- Buoyant Force: {buoyantForceN:0.##} N");
    Console.WriteLine($"-- Wall Thickness: {sphereSize.ThicknessM:0.#####} M");
    Console.WriteLine($"-- Orb Volume: {sphereSize.VolumeM3:0.##} M3");
    Console.WriteLine($"-- Orb Mass: {sphereSize.MassKG:0.##} KG");
    Console.WriteLine($"-- Material Cost: {sphereCost.Material:C}");
    Console.WriteLine($"-- Annual License Cost: {sphereCost.Commercial:C}");
}

_ = Console.ReadKey();