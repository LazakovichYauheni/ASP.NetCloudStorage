using System;

namespace ASPMVCIdentity.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string ErrorMessage { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}