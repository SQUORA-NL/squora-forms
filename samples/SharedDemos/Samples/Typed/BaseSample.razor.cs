using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Components.Factories;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SharedDemos.Samples.Typed
{
    public partial class BaseSample : SampleBasePage
    {
        [Inject]
        ISnackbar? Snackbar { get; set; } 

        private void Submit()
        {
            if(Snackbar != null)
            {
                Snackbar.Add("Form is Submitted", Severity.Normal, item => item.VisibleStateDuration = 1000);
            }            
        }

        private void update()
        {
            StateHasChanged();
        }

        TestData data = new TestData() { name = "demo", birthDate = "none" };

        private DynamicFormModel model = FormFactory.GroupLayout(gr => gr
                .WithParameter("style", "")
                ,
                 FormFactory.StringComponent("voornaam", "$.name", name => name
                       .WithMinimumLength(7)
                       .WithMaximumLength(14)
                       .WithInvalidMessage("Name should be at least {MinimumLength} and max {MaximumLength} long!")
                       ),
                  FormFactory.SubmitAction("Submit form - from the middle of the form"),
                 FormFactory.StringComponent("Date", "$.birthDate", birthdate => birthdate
                       .WithFormat("dd-MM-YYYY")
                       )
                
               );


        public class TestData
        {
            public string name { get; set; }
            public string birthDate { get; set; }
        }

        public string documentation = "";

       
    }
}
