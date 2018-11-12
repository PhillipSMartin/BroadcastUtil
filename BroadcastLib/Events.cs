using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BroadcastLib
{
    public delegate void MessageDelegate<T>(T message);
    public delegate void ClosedEventHandler(object sender, ClosedEventArgs e);
 }
