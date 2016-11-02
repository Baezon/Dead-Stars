using UnityEngine;
using System.Collections;

public class BellCurve : MonoBehaviour {

	public static float random(float mean,float stdDev){
		return mean+stdDev*NextGaussianDouble();
	}

	public static float random(float mean,float stdDev,float bound){
		float r;
		do {
			r = random (mean, stdDev);
		} while(r>mean+bound || r<mean-bound);
		return r;
	}

	public static float random(float mean, float stdDev, float lowbound, float highbound)
	{
		float r;
		do
		{
			r = random(mean, stdDev);
		} while (r > mean + highbound || r < mean - lowbound);
		return r;
	}


	public static float NextGaussianDouble()
	{
		float u, v, S;
		
		do
		{
			u = 2f * Random.value - 1f;
			v = 2f * Random.value - 1f;
			S = u * u + v * v;
		}
		while (S >= 1f);
		
		float fac = Mathf.Sqrt(-2f * Mathf.Log(S) / S);
		return u * fac;
	}
}
