using System;
using System.Collections;
using System.Collections.Generic;

public class UnitTemplate : Object {

    public UnitStateTemplate initialState { get; private set; }

    public UnitTemplate Copy() {
        UnitTemplate copy = new UnitTemplate(initialState.Copy());
        return copy;
    }

    public UnitTemplate() {
        initialState = new UnitStateTemplate();
    }

    public UnitTemplate(UnitStateTemplate initialStateTemplate) {
        initialState = initialStateTemplate;
    }

}
