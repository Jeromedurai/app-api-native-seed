using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tenant.Query.Model.Order
{
    /// <summary>
    /// Enumeration for order statuses
    /// </summary>
    public enum OrderStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Confirmed")]
        Confirmed = 1,

        [Display(Name = "Processing")]
        Processing = 2,

        [Display(Name = "Shipped")]
        Shipped = 3,

        [Display(Name = "Delivered")]
        Delivered = 4,

        [Display(Name = "Cancelled")]
        Cancelled = 5,

        [Display(Name = "Returned")]
        Returned = 6,

        [Display(Name = "Refunded")]
        Refunded = 7
    }

    /// <summary>
    /// Enumeration for payment failure reasons
    /// </summary>
    public enum PaymentFailureReason
    {
        [Display(Name = "Cancelled")]
        Cancelled = 0,

        [Display(Name = "Failed")]
        Failed = 1
    }

    /// <summary>
    /// Helper class for enum conversions
    /// </summary>
    public static class OrderStatusExtensions
    {
        /// <summary>
        /// Convert OrderStatus enum to string representation
        /// </summary>
        public static string ToDisplayString(this OrderStatus status)
        {
            var attribute = status
                .GetType()
                .GetMember(status.ToString())[0]
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? status.ToString();
        }

        /// <summary>
        /// Convert PaymentFailureReason enum to string representation
        /// </summary>
        public static string ToDisplayString(this PaymentFailureReason reason)
        {
            var attribute = reason
                .GetType()
                .GetMember(reason.ToString())[0]
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? reason.ToString();
        }

        /// <summary>
        /// Try to parse string to OrderStatus enum
        /// </summary>
        public static bool TryParseOrderStatus(string status, out OrderStatus result)
        {
            result = OrderStatus.Pending;
            if (string.IsNullOrWhiteSpace(status))
                return false;

            // Try direct enum parse
            if (System.Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                result = parsedStatus;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to parse string to PaymentFailureReason enum
        /// </summary>
        public static bool TryParsePaymentFailureReason(string reason, out PaymentFailureReason result)
        {
            result = PaymentFailureReason.Failed;
            if (string.IsNullOrWhiteSpace(reason))
                return false;

            // Try direct enum parse
            if (System.Enum.TryParse<PaymentFailureReason>(reason, ignoreCase: true, out var parsedReason))
            {
                result = parsedReason;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get all valid OrderStatus values as display names
        /// </summary>
        public static IEnumerable<string> GetValidOrderStatuses()
        {
            return System.Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(s => s.ToDisplayString());
        }

        /// <summary>
        /// Get all valid PaymentFailureReason values as display names
        /// </summary>
        public static IEnumerable<string> GetValidPaymentFailureReasons()
        {
            return System.Enum.GetValues(typeof(PaymentFailureReason))
                .Cast<PaymentFailureReason>()
                .Select(r => r.ToDisplayString());
        }
    }
}
