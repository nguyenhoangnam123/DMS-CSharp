using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Entities
{
    public class Mail
    {
        public long Id { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long RetryCount { get; set; }
        public string Error { get; set; }
        public Mail() { }
    }

    public class MailFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Recipients { get; set; }
        public StringFilter Subject { get; set; }
        public StringFilter Body { get; set; }
        public LongFilter RetryCount { get; set; }

    }

    public enum MailOrder
    {
        Rece
    }
}
