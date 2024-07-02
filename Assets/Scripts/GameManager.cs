using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(ExecuteGameLoop());
    }

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine(StartGameRoutine());
        yield return StartCoroutine(PlayGameRoutine());

        // wait for board to refill
        yield return StartCoroutine(WaitForBoardRoutine(0.5f));

        yield return StartCoroutine(EndGameRoutine());
    }
    IEnumerator StartGameRoutine()
    {
        yield return null;
    }

    IEnumerator PlayGameRoutine()
    {
        yield return null;
    }

    IEnumerator WaitForBoardRoutine(float v)
    {
        yield return null;
    }

    IEnumerator EndGameRoutine()
    {
        yield return null;
    }

}
