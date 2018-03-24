using System;
using System.Collections.Generic;
using System.Reflection;

public partial class Unit {

    private List<Effect> ongoingEffects;

    public void ApplyEffect(Effect e) {
        if (e.isPermanent) {

            Type type = this.GetType();
            PropertyInfo prop = type.GetProperty(e.keyPath);
            float value = e.b;

            if (e.a != 0) {
                float oldValue = (float)prop.GetValue(this, null);
                value += oldValue * e.a;
            }

            prop.SetValue(this, value, null);
        }
    }
} 