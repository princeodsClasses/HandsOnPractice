using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponModelInfo : MonoBehaviour
{
	[SerializeField]
	Transform _tHandDummy;

	public Transform GetHandDummyTransform() => _tHandDummy;
}
