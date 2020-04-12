using System;
using PluginScribens.Common.Enums;

namespace PluginScribens.Common.IdentityChecker
{
    public class Identity
    {
        public IdentityStatus Status { get; set; }
        public  string Username { get; set; }
        public  string Password { get; set; }
        public  string Email { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? LastSubscriptionExpiredDate { get; set; }
        public string Options { get; set; }

        public string SecurePassword
        {
            get
            {
                var result = "";
                foreach (char c in Password)
                    result += '*';

                return result;
            }
        }

        public bool IsValid()
        {
            return Status == IdentityStatus.True || Status == IdentityStatus.TimeExpired || Status == IdentityStatus.InscriptionSimple;
        }

        public bool IsExpired()
        {
            return Status == IdentityStatus.TimeExpired || (Status == IdentityStatus.InscriptionSimple && HasExpiredDate())
                || ExpiredDate == null;
        }

        private bool HasExpiredDate()
        {
            return ExpiredDate.HasValue || LastSubscriptionExpiredDate.HasValue;
        }
    }
}
