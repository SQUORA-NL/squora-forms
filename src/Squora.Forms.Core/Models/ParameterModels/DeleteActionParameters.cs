namespace Squora.Forms.Core.Models.ParameterModels
{
    public class DeleteActionParameters
    {
        public static ParameterList Set(string label, string path)
        {
            return new ParameterList()
            {
                { ParameterNames.Label , label },
                { ParameterNames.Path, path},
            };
        }
    }
}
