using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace UserActor
{
    [DataContract]
    public class UserActorState
    {
        public UserActorState(bool isEmailEnabled)
        {
            IsEmailEnabled = isEmailEnabled;
        }
        public bool IsEmailEnabled { get; set; }
    }
}
