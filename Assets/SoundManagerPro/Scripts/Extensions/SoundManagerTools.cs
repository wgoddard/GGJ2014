using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SoundManagerTools {
	static readonly System.Random random = new System.Random();
	public static void Shuffle<T> ( ref List<T> theList )
	{
		int n = theList.Count;
		while (n > 1)
		{
			n--;
			int k = random.Next(n + 1);
			T val = theList[k];
			theList[k] = theList[n];
			theList[n] = val;
		}
	}
	
	public static void make2D ( ref AudioSource theAudioSource )
	{
		theAudioSource.panLevel = 0f;
	}
	
	public static void make3D ( ref AudioSource theAudioSource )
	{
		theAudioSource.panLevel = 1f;
	}
}
