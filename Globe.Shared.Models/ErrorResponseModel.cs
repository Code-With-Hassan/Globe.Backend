using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Globe.Shared.Models
{
    /// <summary>
    /// In case of exception this is the model client gets 
    /// </summary>
    public class ErrorResponseModel
    {
        /// <summary>
        /// type of exception (Exception, SqlException, BussinessException, ModelStateError)
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// Error Description, usually the exception's message
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// List of Posted model's Errors. When ErrorType = 'ModelStateError'
        /// </summary>
        public ModelStateDictionary ModelState { get; set; }

    }
}
