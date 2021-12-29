using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Validation1
{
    [ServiceContract]
    public interface IValidation
    {
        [OperationContract]
        Task<bool> ValidateAsync(string ime, string prezime, string book, int id);
    }
}
