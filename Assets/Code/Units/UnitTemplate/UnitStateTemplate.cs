using System.Collections;
using System.Collections.Generic;
using System;

public class UnitStateTemplate : Object {

    public UnitParametersTemplate parametersTemplate { get; private set; }
    private List<UnitStateTemplate> accessibleStates;

    public UnitStateTemplate() {
        parametersTemplate = new UnitParametersTemplate();
        accessibleStates = new List<UnitStateTemplate>();
    }

}
