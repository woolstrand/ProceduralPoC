using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UnitStateTemplate {

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

    public UnitStateTemplate Copy() {
        UnitStateTemplate copy = new UnitStateTemplate();
        copy.parametersTemplate = parametersTemplate.Copy();
        copy.accessibleStates = accessibleStates.ConvertAll(state => state.Copy());
        return copy;
    }

}
