using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using IdentityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public static class UserAuthStateExtensions
    {
        public static UserAuthState Clone(this UserAuthState userAuthState)
        {
            return new UserAuthState
            {
                PrincipalId = userAuthState.PrincipalId,
                IsAuthorised = userAuthState.IsAuthorised,
                MailMessages = new List<MailMessage>(userAuthState.MailMessages.Select(x => new MailMessage
                {
                    From = x.From,
                    To = x.To,
                    MsgId = x.MsgId,
                    ReadState = x.ReadState,
                    ReceivedDate = x.ReceivedDate,
                    Subject = x.Subject,
                    Type = x.Type
                }))
            };
        }
    }
}
