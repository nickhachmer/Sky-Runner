using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "PhysicsDatabase", menuName = "Databases/PhysicsDatabase")]
public class PhysicsDatabase : ScriptableObject
{
	public string objectName = "New MyScriptableObject";
    public bool colorIsRandom = false;
}