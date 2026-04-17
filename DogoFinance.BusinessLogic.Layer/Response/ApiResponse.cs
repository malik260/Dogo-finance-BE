using System.Text.Json;

namespace DogoFinance.BusinessLogic.Layer.Response
{
    /// <summary>
    /// Standard API response model used across the entire Business Logic Layer,
    /// modeled after Fintrak's ApiResponse.
    /// </summary>
    public class ApiResponse
    {
        public string? Message { get; set; }
        public object? Data { get; set; }
        public int Status { get; set; }
        public bool Boolean { get; set; }
        public bool Success { get; set; }
        public int? TotalRows { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);

        private void SetMessageAndStatus(string message, int status)
        {
            Message = message;
            Status = status;
        }

        private void SetMessageAndState(string message, bool state)
        {
            Message = message;
            Boolean = state;
            Success = state;
            Status = state ? 200 : 400;
        }

        public void SetError(string message, int status)
        {
            SetMessageAndStatus(message, status);
            Boolean = false;
            Success = false;
            Data = null;
        }

        public void SetMessage(string message, int status)
        {
            SetMessageAndStatus(message, status);
            Data = null;
        }

        public void SetMessage(string message, bool state)
        {
            SetMessageAndState(message, state);
            Data = null;
        }

        public void SetMessage(string message, bool state, object relay)
        {
            SetMessageAndState(message, state);
            Data = relay;
        }

        public void SetMessage(string message, int status, object relay)
        {
            SetMessageAndStatus(message, status);
            Data = relay;
        }
    }
}
