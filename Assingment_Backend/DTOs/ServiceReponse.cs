namespace Assignment_Backend.DTOs
{
    public class ServiceResponse

    {
        public bool Isuccess { get; set; }
        public string Message { get; set; }

        public ServiceResponse(bool status, string Message)
        {
            Isuccess = status;
            this.Message = Message;
        }
    }
}
