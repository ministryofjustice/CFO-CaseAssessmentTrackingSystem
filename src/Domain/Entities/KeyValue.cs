using System.ComponentModel;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Events;

namespace Cfo.Cats.Domain.Entities
{
    public class KeyValue : BaseAuditableEntity<int>, IAuditTrial
    {

        private KeyValue()
        {
        }

        public Picklist Name { get; set; } = Picklist.ReferralSource;
        public string? Value { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
    }

    public enum Picklist
    {
        [Description("Referral Source")]
        ReferralSource = 0,

        [Description("QA Return Reason")]
        QaReturnReason = 1,

        [Description("Education Level")]
        EducationLevel = 2,

        [Description("Employment Type")]
        EmploymentType = 3,

        [Description("Occupation")]
        Occupation = 4,

        [Description("Salary Frequency")]
        SalaryFrequency = 5,

        [Description("Offence Code")]
        OffenceCode = 6
    }

    public sealed class KeyValueCreatedDomainEvent(KeyValue entity) : CreatedDomainEvent<KeyValue>(entity)
    {
    }

    public sealed class KeyValueUpdatedDomainEvent(KeyValue entity) : UpdatedDomainEvent<KeyValue>(entity)
    {
    }
}