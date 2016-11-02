
using UnityEngine;
using System.Collections.Generic;
public class Star : CBody
{
    /// Stellar surface temperature in kelvin
	public float temp;

	public override string getPlanetSprite(){
		return "Star";
	}

	public Star (float _aphelion, float _angle, float _size, float _density, float _temp, StarSystem _system) 
		: base(_aphelion, _angle, _size, _density, null,_system)
	{
		temp = _temp;
	}

    /// The center of the "goldilocks zone" for this star, in AU
    public float goldilocksCenter
    {
        get { return 6.119e-14f * Mathf.Sqrt(luminosity); }
    }

    /// luminosity in watts
    public float luminosity
    {
        get {
            float x = size * temp * temp;
            return .7125f * x * x;
        }
    }

    public override IEnumerable<CBody> starChildren
    {
        get
        {
            foreach (var cBody in system.cBodies)
            {
                if (cBody is Planet) { 
                yield return cBody;
                }
            }
        }
    }
}

