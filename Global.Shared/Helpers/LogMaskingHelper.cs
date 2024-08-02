using Globe.Shared.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Globe.Shared.Helpers
{
    public class LogMaskingHelper
    {
        private readonly ILogger<LogMaskingHelper> _logger;

        private readonly LoggingSettingsModel _loggingSettings;
        public LogMaskingHelper(ILogger<LogMaskingHelper> logger, LoggingSettingsModel loggingSettings)
        {
            _logger = logger;
            _loggingSettings = loggingSettings;
        }

        public string MaskLog(string requestPath, string unmaskedBody)
        {
            try
            {
                if (_loggingSettings.Enable)
                {
                    var request = _loggingSettings.Requests.FirstOrDefault(x => x.Url.ToLower() == requestPath.ToLower());
                    if (request != null)
                    {
                        List<JObject> jsonObjects = new List<JObject>();
                        List<string> maskedJsonStrings = new List<string>();
                        JArray jsonArray = new JArray();
                        if (unmaskedBody.TrimStart().StartsWith("[")) //response is a JSON array
                        {
                            jsonArray = JArray.Parse(unmaskedBody);
                            var jToken = jsonArray.First;
                            while (jToken.Next != null)
                            {
                                jsonObjects.Add(JObject.FromObject(jToken));
                                jToken = jToken.Next;
                            }
                        }
                        else
                            jsonObjects.Add(JObject.Parse(unmaskedBody));

                        //jsonObject = JObject.Parse(unmaskedBody); //this object will be used to create another one

                        //Create a new list to iterate over
                        int j = 0;
                        List<JObject> jsonObjectsCopy = new List<JObject>();
                        foreach (var jsonObject in jsonObjects)
                            jsonObjectsCopy.Add(jsonObject);

                        foreach (var jsonObject in jsonObjectsCopy)
                        {
                            var jsonObjectLowerCasePropertyNames = new JObject();
                            #region Make all property names lower case
                            foreach (var jObjProp in jsonObject.Properties())
                                jsonObjectLowerCasePropertyNames.Add(new JProperty(jObjProp.Name.ToLower(), jObjProp.Value));
                            #endregion

                            foreach (var propertyToBeMasked in request.Properties)
                            {
                                var property = jsonObjectLowerCasePropertyNames.Property(propertyToBeMasked.PropertyName.ToLower());
                                
                                if (property != null)
                                {
                                    if (propertyToBeMasked.PartialMask)
                                    {
                                        if (propertyToBeMasked.StartIndex <= propertyToBeMasked.EndIndex)
                                        {
                                            if (property.Value.ToString().Length <= propertyToBeMasked.EndIndex)
                                            {
                                                _logger.LogWarning("Property \"{property}\": masking EndIndex is {idx}, but property length is {length}! Setting to {end}.",
                                                    propertyToBeMasked.PropertyName, propertyToBeMasked.EndIndex, property.Value.ToString().Length, property.Value.ToString().Length - 1);

                                                propertyToBeMasked.EndIndex = property.Value.ToString().Length - 1;
                                            }
                                            char[] propertyCharArray = property.Value.ToString().ToCharArray();
                                            #region Check for negative indices
                                            if (propertyToBeMasked.StartIndex < 0)
                                            {
                                                _logger.LogWarning("Property \"{property}\": masking StartIndex is {idx}, which is less than 0! Setting to 0.",
                                                    propertyToBeMasked.PropertyName, propertyToBeMasked.StartIndex);
                                                propertyToBeMasked.StartIndex = 0;
                                            }
                                            if (propertyToBeMasked.EndIndex < 0)
                                            {
                                                _logger.LogWarning("Property \"{property}\": masking EndIndex is {idx}, which is less than 0! Setting to 0.",
                                                    propertyToBeMasked.PropertyName, propertyToBeMasked.EndIndex);
                                                propertyToBeMasked.EndIndex = 0;
                                            }
                                            #endregion

                                            for (int i = propertyToBeMasked.StartIndex; i < propertyToBeMasked.EndIndex + 1; i++)
                                            {
                                                propertyCharArray[i] = '*';
                                            }
                                            property.Value = new string(propertyCharArray);
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Property \"{property}\": masking StartIndex = {startIdx}, which greater than EndIndex = {endIdx}! Will mask fully.",
                                                    propertyToBeMasked.PropertyName, propertyToBeMasked.StartIndex, propertyToBeMasked.EndIndex);
                                            property.Value = "******";
                                        }
                                    }
                                    else
                                    {
                                        property.Value = "******";
                                    }
                                }
                            }
                            #region Assign masked values to original JObject
                            foreach (var prop in jsonObjectLowerCasePropertyNames.Properties())
                            {
                                var originalProperty = jsonObject.Properties()
                                    .FirstOrDefault(x => x.Name.ToLower() == prop.Name);
                                if (originalProperty != null)
                                    originalProperty.Value = prop.Value;
                            }
                            #endregion

                            jsonObjects[j] = jsonObject;
                            j++;
                        }
                        return jsonObjects.Count() > 1 ? Newtonsoft.Json.JsonConvert.SerializeObject(jsonObjects) : Newtonsoft.Json.JsonConvert.SerializeObject(jsonObjects[0]);
                    }
                    else
                        return unmaskedBody;
                }
                else
                    return unmaskedBody;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Exception caught while masking the body for logging. " +
                    "Will mask fully. Exception: {ex}", ex);
                return "******";
            }            
        }
    }
}
