namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class TeamView : ComponentBase
    {
        private bool isSelected;

        [Parameter]
        public TeamModel Team { get; set; }

        [Parameter]
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                }
            }
        }

        [Parameter]
        public EventCallback<bool> IsSelectedChanged { get; set; }

        private Task OnSelectionChange(ChangeEventArgs e)
        {
            isSelected = (bool)e.Value;

            return IsSelectedChanged.InvokeAsync(IsSelected);
        }
    }
}
