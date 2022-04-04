using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary.Models
{
    /// <summary>
    /// Response message used to reply back to our web API requests.
    /// </summary>
    public class ResponseMessage
    {

        #region Constructors

        /// <summary>
        /// Default constructor. 
        /// Need for serialization purposes.
        /// </summary>
        public ResponseMessage()
        {
        }

        /// <summary>
        /// Fields constructor.
        /// </summary>
        public ResponseMessage(bool state, string message, object data)
        {
            State = state;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Clone/Copy constructor.
        /// </summary>
        /// <param name="instance">The object to clone from.</param>
        public ResponseMessage(ResponseMessage instance)
            : this(instance.State, instance.Message, instance.Data)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// States whether the response contains data or not.
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public bool State { get; set; }

        /// <summary>
        /// Server message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Data requested by the client.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Data { get; set; }

        #endregion

    }
}
