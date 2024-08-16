using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionClass
{
#if UNITY_STANDALONE_OSX
	public const string DEVICE_TYPE = "mac";
#elif UNITY_STANDALONE_WIN
	public const string DEVICE_TYPE = "win";
#elif UNITY_WEBGL
	public const string DEVICE_TYPE = "web";
#endif
	public const string BUNDLE_VERSION = "0.1";

	public static string FullVersionStr { get { return DEVICE_TYPE + BUNDLE_VERSION; } }

	// NOTE: if < 0 means BUNDLE_VERSION is smaller than otherBundleVersion.
	public static int OrdinalCompareBundleVersion(string inOtherBundleVersion)
	{
		return string.CompareOrdinal(BUNDLE_VERSION, inOtherBundleVersion);
	}
}
