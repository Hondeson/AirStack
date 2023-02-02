namespace AirStack.Core.Model.API
{
    public class UpdateItemDTO
    {
        /// <summary>
        /// Prázdný konstruktor třeba pro deserializaci json
        /// </summary>
        public UpdateItemDTO()
        {

        }

        public UpdateItemDTO(string code, StatusEnum state)
        {
            Code = code;
            ActualStatus = state;
        }

        public string Code { get; set; }
        public StatusEnum ActualStatus { get; set; }
    }
}
