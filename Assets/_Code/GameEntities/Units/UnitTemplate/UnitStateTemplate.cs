using System.Collections;
using System.Collections.Generic;
using System;

public class UnitStateTemplate : Object {

    public UnitParametersTemplate parametersTemplate { get; private set; }
    public List<UnitStateTemplate> accessibleStates { get; private set; }

    public UnitStateTemplate() {
        parametersTemplate = new UnitParametersTemplate();
        accessibleStates = new List<UnitStateTemplate>();
    }

    public UnitStateTemplate(UnitParametersTemplate parametersTemplate, List<UnitStateTemplate> accessibleStates = null) {
        this.parametersTemplate = parametersTemplate;

        if (accessibleStates != null) {
            this.accessibleStates = new List<UnitStateTemplate>(accessibleStates);
        } else {
            this.accessibleStates = new List<UnitStateTemplate>();
        }
    }

}
