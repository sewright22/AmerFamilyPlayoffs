namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Matchup : ComponentBase
    {
        private bool homeIsWinner;
        private bool awayIsWinner;

        [Parameter]
        public GameModel Game { get; set; }

        [Parameter] public Action Save { get; set; }

        public bool HomeIsWinner
        {
            get
            {
                return this.homeIsWinner;
            }

            set
            {
                if (this.homeIsWinner != value)
                {
                    this.homeIsWinner = value;

                    if (this.homeIsWinner)
                    {
                        AwayIsWinner = false;
                    }

                    UpdateWinner();
                }
            }
        }

        public bool AwayIsWinner
        {
            get
            {
                return this.awayIsWinner;
            }
            set
            {
                if (this.awayIsWinner != value)
                {
                    this.awayIsWinner = value;

                    if (this.awayIsWinner)
                    {
                        HomeIsWinner = false;
                    }

                    UpdateWinner();
                }
            }
        }

        public void UpdateWinner()
        {
            if (AwayIsWinner)
            {
                Game.Winner = Game.AwayTeam;
            }
            else if (HomeIsWinner)
            {
                Game.Winner = Game.HomeTeam;
            }
            else
            {
                Game.Winner = null;
            }

            Save();
        }
    }
}
