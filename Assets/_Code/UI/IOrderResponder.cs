using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IOrderResponder {
    void DidSelectMove();
    void DidSelectStop();
    void DidSelectAttack();
}
