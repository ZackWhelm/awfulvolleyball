using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanControlCamera
{
	void SetTarget(ref Transform target);
}
