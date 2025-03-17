namespace Assignment_Backend.DTOs
{
    public class ReposResponse
    {
        public bool Isuccess { get; set; }
        public string Message {  get; set; } 

        public ReposResponse(bool status , string Message) 
        {
            Isuccess = status;
            this.Message = Message; 
        }

    }
}
