using onmov200.model;

namespace onmov200
{
    public class OMError
    {
        private ActivityHeader activity;

        private string message;
        
        public string Message => message;

        public ActivityHeader Activity => activity;

        public string Message1 => Message;

        public string ErrorMessage => activity == null ? Message : $"{Activity} [{Activity.Name}] : {Message}"; 

        public OMError(ActivityHeader activity, string message)
        {
            this.activity = activity;
            this.message = message;
        }
    }
}