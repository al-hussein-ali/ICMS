using ICMS.Application.Interfaces.Services;
using ICMS.Application;
using Microsoft.Extensions.Localization;

namespace ICMS.Infrastructure.ExternalServices
{
    public class ResourceLocalizer : ILocalizer
    {
        private readonly IStringLocalizer<ExceptionsResource> _exceptions;
        private readonly IStringLocalizer<ValidationResource> _validation;
        private readonly IStringLocalizer<MessagesResource> _messages;

        public ResourceLocalizer(
            IStringLocalizer<ExceptionsResource> exceptions,
            IStringLocalizer<ValidationResource> validation,
            IStringLocalizer<MessagesResource> messages)
        {
            _exceptions = exceptions;
            _validation = validation;
            _messages = messages;
        }

        public string this[string key]
        {
            get
            {
                var val = _exceptions[key];
                if (!val.ResourceNotFound) return val;

                val = _validation[key];
                if (!val.ResourceNotFound) return val;

                val = _messages[key];
                if (!val.ResourceNotFound) return val;

                return key;
            }
        }

        public string this[string key, params object[] args]
        {
            get
            {
                var val = _exceptions[key, args];
                if (!val.ResourceNotFound) return val;

                val = _validation[key, args];
                if (!val.ResourceNotFound) return val;

                val = _messages[key, args];
                if (!val.ResourceNotFound) return val;

                return string.Format(key, args);
            }
        }
    }
}
