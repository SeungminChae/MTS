using System.Xml.Serialization;

namespace MarysToyStore.Models
{
    [XmlRoot("AddressValidateResponse")]
    public class UspsAddressValidateResponse
    {
        public UspsAddress Address { get; set; }
    }
}