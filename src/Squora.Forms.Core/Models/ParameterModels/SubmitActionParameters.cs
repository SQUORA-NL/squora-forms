namespace Squora.Forms.Core.Models.ParameterModels
{
    public class SubmitActionParameters
    {
        public static ParameterList Set(string label)
        {
            return new ParameterList()
            {
                { ParameterNames.Label , label },
            };
        }
    }
}
