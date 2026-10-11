namespace Squora.Forms.Core.Models.ParameterModels
{
    public class BaseParameters
    {
        public static new ParameterList Set(string label, string path)
        {
            return new ParameterList()
            {
                { ParameterNames.Label , label },
                { ParameterNames.Path, path},
            };
        }

        public static new ParameterList Set(string label, string path, string style)
        {
            return new ParameterList()
            {
                { ParameterNames.Label , label },
                { ParameterNames.Style , style },
                { ParameterNames.Path, path},
            };
        }
    }
}
