using System;
using UnityEngine;

//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

void MyFunction_float(float Time, float firstTime = 0, float Out)
{
    if (firstTime == 0)
    {
        firstTime = Time;
    }

    Out = firstTime;
}
#endif //MYHLSLINCLUDE_INCLUDED

