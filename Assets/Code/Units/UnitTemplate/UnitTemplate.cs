using System;
using System.Collections;
using System.Collections.Generic;

public class UnitTemplate : Object {

    public UnitTemplate() {
        initialState = new UnitStateTemplate();
    }
        
    public UnitStateTemplate initialState { get; private set; }
	
}
