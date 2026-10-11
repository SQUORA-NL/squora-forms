namespace Squora.Forms.Core.Models.ParameterModels
{
    public class AddActionParameters
    {
        public static ParameterList Set(string label, string path, object defaultValue)
        {
            return new ParameterList()
            {
                { ParameterNames.Label , label },
                { ParameterNames.Path, path},
                { ParameterNames.DefaultValue, defaultValue}
            };
        }
    }
}
