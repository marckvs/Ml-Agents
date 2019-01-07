using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboXAcademia : Academy {
    public static float separacionObjeto;

    public override void AcademyReset()
    {
        separacionObjeto = resetParameters["objectSeparation"];
        
    }
    public override void InitializeAcademy()
    {
        Physics.gravity *= 5f;
    }
}
