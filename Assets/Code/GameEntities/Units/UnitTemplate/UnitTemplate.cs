using System;
using System.Collections;
using System.Collections.Generic;

public class UnitTemplate : Object {

    public UnitStateTemplate initialState { get; private set; }

    public UnitTemplate() {
        initialState = new UnitStateTemplate();
    }

    public UnitTemplate(UnitStateTemplate initialStateTemplate) {
        initialState = initialStateTemplate;
    }

}
