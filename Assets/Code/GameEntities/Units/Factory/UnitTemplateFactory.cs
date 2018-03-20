using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTemplateFactory  {

    public static UnitTemplate defaultUnitTemplate() {
        UnitMovementSettings movement = new UnitMovementSettings(5.0f, 1.0f, 0.1f);
        UnitParametersTemplate parameters = new UnitParametersTemplate(movement);
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }
}
