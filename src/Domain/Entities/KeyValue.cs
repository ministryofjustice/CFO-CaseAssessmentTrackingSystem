using System.ComponentModel;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities
{
    public class KeyValue : BaseAuditableEntity<int>, IAuditTrial
    {
        public Picklist Name { get; set; } = Picklist.ReferralSource;
        public string? Value { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
    }

    public enum Picklist
    {
        [Description("Referral Source")]
        ReferralSource = 0

    }

}