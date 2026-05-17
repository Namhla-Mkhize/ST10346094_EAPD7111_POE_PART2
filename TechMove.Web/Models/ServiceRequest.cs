using System.ComponentModel.DataAnnotations;

namespace TechMove.Web.Models
{
    public enum ServiceRequestStatus
    {
        Open,
        InProgress,
        Resolved,
        Cancelled
    }
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Cost (USD)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0")]
        public decimal CostUSD { get; set; }

        [Display(Name = "Cost (ZAR)")]
        public decimal CostZAR { get; set; }

        [Display(Name = "Exchange Rate Used")]
        public decimal ExchangeRateUsed { get; set; }

        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Open;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
