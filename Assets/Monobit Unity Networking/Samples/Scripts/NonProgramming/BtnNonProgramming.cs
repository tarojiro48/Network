using UnityEngine;
using System.Collections;

/// <summary>
/// NoneProgrammingに遷移する
/// </summary>
public class BtnNonProgramming : MonoBehaviour
{
	/// <summary>
	/// NoneProgrammingに遷移する
	/// </summary>
	public void SceneLoad()
	{
		Application.LoadLevel("NoneProgramming");
	}
}
