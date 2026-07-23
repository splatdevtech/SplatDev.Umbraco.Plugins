namespace SplatDev.Payments.MercadoPago.Models
{

    using SplatDev.Payments.Interfaces;
    public struct Shipment : IShipment
    {
        
        public Address ReceiverAddress { get; set; }
    }
}
